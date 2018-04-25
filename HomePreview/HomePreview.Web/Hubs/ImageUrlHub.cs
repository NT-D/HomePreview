using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace HomePreview.Web.Hubs
{
    //[HubName("ImageUrl")]
    public class ImageUrlHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string text)
        {
            Clients.All.Receive(text); // すべてのクライアントに定義されている Receive メソッドを呼び出す
        }
        /*
         * Clients の人たち
         * 1. Clients.All -> (SignalRで管理している)全てのクライアント
         * 2. Clients.Caller -> 自分のみ
         * 3. Clients.Others -> 自分以外のすべてのクライアント
         * 4. Clients.Group(string groupName) メソッド -> 指定したグループ名に属するクライアント
         */
    }
}