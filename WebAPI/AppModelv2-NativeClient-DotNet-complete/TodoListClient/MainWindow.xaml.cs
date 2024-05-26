// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// 引入所需的命名空間
using Microsoft.Identity.Client; // 使用 Microsoft 身份驗證庫
using Newtonsoft.Json; // 用於 JSON 序列化和反序列化
using System.Collections.Generic; // 支持集合類
using System.Configuration; // 讀取配置文件
using System.Diagnostics; // 提供調試支持
// 接下來的 using 語句是為了這個範例特別添加的。
using System.Globalization; // 支持文化相關的功能
using System.IO; // 文件輸入輸出
using System.Linq; // 提供對集合的查詢功能
using System.Net.Http; // 處理 HTTP 請求和響應
using System.Net.Http.Headers; // 處理 HTTP 標頭
using System.Reflection; // 處理程序集
using System.Text; // 支持文本處理
using System.Threading.Tasks; // 支持異步編程
using System.Windows; // 支持 Windows Presentation Foundation (WPF) 應用程式

namespace TodoListClient
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        // 定義與 Azure AD 身份驗證相關的配置變量
        private static readonly string AadInstance = ConfigurationManager.AppSettings["ida:AADInstance"]; // Azure AD 實例
        private static readonly string Tenant = ConfigurationManager.AppSettings["ida:Tenant"]; // 租戶名稱
        private static readonly string ClientId = ConfigurationManager.AppSettings["ida:ClientId"]; // 應用程式 ID

        private static readonly string Authority = string.Format(CultureInfo.InvariantCulture, AadInstance, Tenant); // 構造權威 URL

        // 定義待辦事項服務的 App ID URI 和 URL
        private static readonly string TodoListScope = ConfigurationManager.AppSettings["TodoListServiceScope"]; // 待辦事項服務的範圍
        private static readonly string TodoListBaseAddress = ConfigurationManager.AppSettings["TodoListServiceBaseAddress"]; // 待辦事項服務的基礎地址
        private static readonly string[] Scopes = { TodoListScope }; // 將待辦事項服務的範圍放入陣列
        private static string TodoListApiAddress // 構造待辦事項 API 的完整地址
        {
            get
            {
                string baseAddress = TodoListBaseAddress;
                return baseAddress.EndsWith("/") ? TodoListBaseAddress + "api/todolist"
                                                 : TodoListBaseAddress + "/api/todolist";
            }
        }

        private readonly HttpClient _httpClient = new HttpClient(); // 用於 HTTP 請求的 HttpClient 實例
        private readonly IPublicClientApplication _app; // MSAL 的公開客戶端應用程式實例

        // 按鈕文字
        const string SignInString = "Sign In"; // 登入按鈕文字
        const string ClearCacheString = "Clear Cache"; // 清除快取按鈕文字

        public MainWindow()
        {
            InitializeComponent(); // 初始化窗口組件
            _app = PublicClientApplicationBuilder.Create(ClientId) // 建立 MSAL 公開客戶端應用程式實例
                .WithAuthority(Authority) // 設定權威
                .WithDefaultRedirectUri() // 使用默認的重定向 URI
                .Build(); // 建立實例

            TokenCacheHelper.EnableSerialization(_app.UserTokenCache); // 啟用 token 快取的序列化
            GetTodoList(); // 獲取待辦事項列表
        }

        // 當應用啟動時，嘗試獲取待辦事項列表
        private void GetTodoList()
        {
            // 根據用戶是否已登入（檢查按鈕文字），決定是否自動獲取待辦事項
            GetTodoList(SignInButton.Content.ToString() != ClearCacheString).ConfigureAwait(false);
        }

        // 異步獲取待辦事項列表
        private async Task GetTodoList(bool isAppStarting)
        {
            // 嘗試從 MSAL 應用緩存中獲取當前的帳戶
            var accounts = (await _app.GetAccountsAsync()).ToList();
            if (!accounts.Any()) // 如果沒有帳戶，表示用戶尚未登入
            {
                SignInButton.Content = SignInString; // 更新按鈕文字提示用戶登入
                return;
            }

            // 如果已經有帳戶，嘗試無需用戶互動地獲取訪問令牌
            AuthenticationResult result = null;
            try
            {
                result = await _app.AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync()
                    .ConfigureAwait(false); // 從快取中獲取或刷新訪問令牌

                Dispatcher.Invoke(() => // 在 UI 線程上執行
                {
                    SignInButton.Content = ClearCacheString; // 更新按鈕為「清除快取」
                    SetUserName(result.Account); // 更新 UI 上顯示的用戶名
                });
            }
            catch (MsalUiRequiredException) // 如果需要用戶互動來獲取令牌
            {
                if (!isAppStarting) // 如果不是應用啟動時的自動獲取，提示用戶需要登入
                {
                    MessageBox.Show("Please sign in to view your To-Do list");
                    SignInButton.Content = SignInString;
                }
            }
            catch (MsalException ex) // 處理其他 MSAL 異常
            {
                // 顯示錯誤訊息
                MessageBox.Show(ex.Message);

                UserName.Content = Properties.Resources.UserNotSignedIn; // 重置用戶名顯示為未登入
                return;
            }

            // 使用獲取到的訪問令牌調用待辦事項服務 API
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = await _httpClient.GetAsync(TodoListApiAddress); // 發送 HTTP GET 請求

            if (response.IsSuccessStatusCode) // 如果請求成功
            {
                // 解析響應內容並更新 UI
                string s = await response.Content.ReadAsStringAsync();
                List<TodoItem> toDoArray = JsonConvert.DeserializeObject<List<TodoItem>>(s);
                Dispatcher.Invoke(() => // 在 UI 線程上執行
                {
                    TodoList.ItemsSource = toDoArray.Select(t => new { t.Title }); // 更新待辦事項列表
                });
            }
            else
            {
                // 如果請求失敗，顯示錯誤訊息
                await DisplayErrorMessage(response);
            }
        }

        // 顯示 HTTP 請求錯誤訊息的異步方法
        private static async Task DisplayErrorMessage(HttpResponseMessage httpResponse)
        {
            // 讀取並處理錯誤訊息
            string failureDescription = await httpResponse.Content.ReadAsStringAsync();
            if (failureDescription.StartsWith("<!DOCTYPE html>")) // 如果錯誤訊息是 HTML 格式
            {
                // 儲存錯誤訊息到本地檔案並打開它
                string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string errorFilePath = Path.Combine(path, "error.html");
                File.WriteAllText(errorFilePath, failureDescription);
                Process.Start(errorFilePath);
            }
            else // 如果錯誤訊息是文本格式
            {
                // 如果響應內容不是 HTML，則直接彈出消息框顯示錯誤信息
                MessageBox.Show($"{httpResponse.ReasonPhrase}\n {failureDescription}", "An error occurred while getting /api/todolist", MessageBoxButton.OK);
            }
        }

        // 處理「新增待辦事項」按鈕點擊事件的方法
        private async void AddTodoItem(object sender, RoutedEventArgs e)
        {
            // 首先獲取當前登錄的帳戶列表
            var accounts = (await _app.GetAccountsAsync()).ToList();

            // 如果沒有用戶登錄，提示用戶先登錄
            if (!accounts.Any())
            {
                MessageBox.Show("Please sign in first");
                return;
            }
            // 檢查待辦事項文本框是否為空，若為空則提示用戶輸入待辦事項
            if (string.IsNullOrEmpty(TodoText.Text))
            {
                MessageBox.Show("Please enter a value for the To Do item name");
                return;
            }

            // 獲取訪問令牌以調用待辦事項服務
            AuthenticationResult result = null;
            try
            {
                result = await _app.AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync()
                    .ConfigureAwait(false); // 從快取中獲取或刷新訪問令牌

                Dispatcher.Invoke(() =>
                {
                    SetUserName(result.Account); // 更新 UI 上顯示的用戶名
                    UserName.Content = Properties.Resources.UserNotSignedIn;
                });
            }
            catch (MsalUiRequiredException)
            {
                // 如果需要用戶互動來獲取令牌，提示用戶重新登錄
                MessageBox.Show("Please re-sign");
                SignInButton.Content = SignInString;
                return;
            }
            catch (MsalException ex)
            {
                // 處理其他 MSAL 異常，顯示錯誤訊息
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "Error Code: " + ex.ErrorCode + "Inner Exception : " + ex.InnerException.Message;
                }

                Dispatcher.Invoke(() =>
                {
                    UserName.Content = Properties.Resources.UserNotSignedIn; // 重置用戶名顯示為未登入
                    MessageBox.Show("Unexpected error: " + message);
                });

                return;
            }

            // 構造待辦事項對象並序列化為 JSON
            TodoItem todoItem = new TodoItem() { Title = TodoText.Text };
            string json = JsonConvert.SerializeObject(todoItem);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // 使用獲得的訪問令牌向待辦事項服務 API 發送 POST 請求，新增待辦事項
            HttpResponseMessage response = await _httpClient.PostAsync(TodoListApiAddress, content);

            // 處理響應
            if (response.IsSuccessStatusCode)
            {
                // 如果請求成功，清空待辦事項輸入框並刷新待辦事項列表
                TodoText.Text = "";
                GetTodoList(); // 重新獲取待辦事項列表
            }
            else
            {
                // 如果請求失敗，顯示錯誤訊息
                await DisplayErrorMessage(response);
            }
        }

        // 處理用戶登錄和登出的方法
        private async void SignIn(object sender = null, RoutedEventArgs args = null)
        {
            // 獲取當前登錄的帳戶列表
            var accounts = (await _app.GetAccountsAsync()).ToList();

            // 如果按鈕顯示為「清除快取」，則執行登出操作，清空待辦事項列表，並清除應用快取
            if (SignInButton.Content.ToString() == ClearCacheString)
            {
                TodoList.ItemsSource = string.Empty; // 清空待辦事項列表

                // clear the cache
                                // 清除快取
                while (accounts.Any()) // 循環遍歷所有帳戶
                {
                    await _app.RemoveAsync(accounts.First()); // 從 MSAL 應用緩存中移除帳戶
                    accounts = (await _app.GetAccountsAsync()).ToList(); // 重新獲取帳戶列表
                }
                // 更新 UI 元素
                SignInButton.Content = SignInString; // 將按鈕文字改為「登入」
                UserName.Content = Properties.Resources.UserNotSignedIn; // 顯示用戶未登入的信息
                return;
            }

            // 如果用戶尚未登入，則執行登入操作
            try
            {
                // 嘗試靜默登入，不需要用戶互動
                var result = await _app.AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                Dispatcher.Invoke(() => // 在 UI 線程上更新 UI 元素
                {
                    SignInButton.Content = ClearCacheString; // 更新按鈕文字為「清除快取」
                    SetUserName(result.Account); // 顯示用戶名
                    GetTodoList(); // 獲取待辦事項列表
                });
            }
            catch (MsalUiRequiredException) // 如果無法靜默登入，則需要用戶交互來完成登入
            {
                try
                {
                    // 強制用戶交互登入
                    var result = await _app.AcquireTokenInteractive(Scopes)
                        .WithAccount(accounts.FirstOrDefault()) // 優先使用已知帳戶
                        .WithPrompt(Prompt.SelectAccount) // 允許用戶選擇帳戶
                        .ExecuteAsync()
                        .ConfigureAwait(false);

                    Dispatcher.Invoke(() => // 在 UI 線程上更新 UI 元素
                    {
                        SignInButton.Content = ClearCacheString; // 更新按鈕文字為「清除快取」
                        SetUserName(result.Account); // 顯示用戶名
                        GetTodoList(); // 獲取待辦事項列表
                    });
                }
                catch (MsalException ex) // 處理 MSAL 異常
                {
                    if (ex.ErrorCode == "access_denied")
                    {
                        // 用戶取消登入，不進行任何操作
                    }
                    else
                    {
                        // 出現未預期錯誤，顯示錯誤訊息
                        string message = ex.Message;
                        if (ex.InnerException != null)
                        {
                            message += "\nError Code: " + ex.ErrorCode + "\nInner Exception: " + ex.InnerException.Message;
                        }
                        MessageBox.Show(message);
                    }

                    Dispatcher.Invoke(() => // 在 UI 線程上更新 UI 元素
                    {
                        UserName.Content = Properties.Resources.UserNotSignedIn; // 重置顯示為用戶未登入
                    });
                }
            }
        }

        // 設置用戶名到 UI 上的方法
        private void SetUserName(IAccount userInfo)
        {
            string userName = null;

            if (userInfo != null) // 如果用戶信息不為空
            {
                userName = userInfo.Username; // 獲取用戶名
            }

            if (userName == null)
                userName = Properties.Resources.UserNotIdentified; // 如果用戶名為空，則顯示未識別的用戶

            UserName.Content = userName; // 更新 UI 上的用戶名
        }
    }
}

