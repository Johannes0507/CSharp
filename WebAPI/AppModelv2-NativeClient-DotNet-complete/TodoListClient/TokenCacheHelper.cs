// 引入必要的命名空間
using System.IO; // 用於文件操作
using System.Security.Cryptography; // 提供加密服務
using Microsoft.Identity.Client; // 包含身份驗證功能

// 定義一個靜態類，用於處理 Token 快取的序列化和反序列化
namespace TodoListClient
{
    static class TokenCacheHelper
    {
        // 快取檔案的路徑，位於執行組件的同目錄下，文件名為執行文件名加上.msalcache.bin
        private static readonly string CacheFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalcache.bin";

        // 一個對象，用於同步訪問快取檔案，避免多線程衝突
        private static readonly object FileLock = new object();

        // 在訪問 Token 快取之前執行的回調方法
        private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock) // 確保同一時間只有一個執行緒可以執行下面的代碼塊
            {
                // 嘗試讀取快取檔案，如果檔案存在則解密並反序列化到 TokenCache
                args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath)
                    ? ProtectedData.Unprotect(File.ReadAllBytes(CacheFilePath),
                                              null,
                                              DataProtectionScope.CurrentUser)
                    : null);
            }
        }

        // 在訪問 Token 快取之後執行的回調方法
        private static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // 如果快取的狀態改變了（例如新增或移除了 Token）
            if (args.HasStateChanged)
            {
                lock (FileLock) // 同上，防止多線程衝突
                {
                    // 將變更後的 Token 快取序列化，加密後寫回檔案
                    File.WriteAllBytes(CacheFilePath,
                                       ProtectedData.Protect(args.TokenCache.SerializeMsalV3(),
                                                             null,
                                                             DataProtectionScope.CurrentUser)
                                      );
                }
            }
        }

        // 啟用 Token 快取的序列化與反序列化
        internal static void EnableSerialization(ITokenCache tokenCache)
        {
            // 設置快取的前置訪問和後置訪問回調方法
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
        }
    }
}
