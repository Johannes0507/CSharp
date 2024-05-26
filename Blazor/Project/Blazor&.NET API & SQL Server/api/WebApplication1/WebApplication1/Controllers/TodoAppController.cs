using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoAppController : ControllerBase
    {
        private IConfiguration _configuration;
        public TodoAppController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetNotes")]
        public JsonResult GetNotes()
        {
            string query = "select * from dbo.Notes";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using(SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using(SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        [Route("AddNotes")]
        public IActionResult AddNotes([FromForm] string newNotes)
        {
            string query = "insert into dbo.Notes (description) values (@newNotes)";
            using (SqlConnection myCon = new SqlConnection(_configuration.GetConnectionString("todoAppDBCon")))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@newNotes", newNotes);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return new JsonResult($"Added Successfully, {rowsAffected} row(s) affected.");
                }
            }
        }

        [HttpDelete]
        [Route("DeleteNotes")]
        public IActionResult DeleteNotes(int id)
        {
            string query = "delete from dbo.Notes where id=@id";
            using (SqlConnection myCon = new SqlConnection(_configuration.GetConnectionString("todoAppDBCon")))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return Ok($"Deleted Successfully, {rowsAffected} row(s) affected.");
                    }
                    else
                    {
                        return NotFound("Note not found.");
                    }
                }
            }
        }

    }
}
