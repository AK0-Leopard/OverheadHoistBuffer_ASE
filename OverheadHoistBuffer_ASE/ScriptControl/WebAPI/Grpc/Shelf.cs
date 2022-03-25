using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.Data;
using CommonMessage.ProtocolFormat.ShelfFun;
using Grpc.Core;
namespace com.mirle.ibg3k0.sc.WebAPI.Grpc
{
    internal class Shelf : shelfGreeter.shelfGreeterBase
    {
        App.SCApplication app;
        public Shelf(App.SCApplication _app)
        {
            app = _app;
        }
        public override Task<replyAllShelfInfo> getAllShelfInfo(Empty empty, ServerCallContext context)
        {
            replyAllShelfInfo result = new replyAllShelfInfo();
            //var allShelfData = app.ShelfDefDao.LoadShelfDef(DBConnection_EF.GetUContext());
            var allShelfData = app.ShelfDefBLL.LoadShelf();
            foreach (var shelfData in allShelfData)
            {
                shelf temp = new shelf();
                temp.BoxId = "";
                temp.Enable = (shelfData.Enable == "Enable") ? true : false;
                temp.ShelfId = shelfData.ShelfID;
                temp.ZoneId = shelfData.ZoneID;
                temp.AdrId = shelfData.ADR_ID;
                temp.CstId = "";
                #region shelf status
                switch (shelfData.ShelfState)
                {
                    case "E":
                        temp.ShelfStatus = shelfStatus.Empty;
                        break;
                    case "S":
                        temp.ShelfStatus = shelfStatus.Store;
                        break;
                    case "I":
                        temp.ShelfStatus = shelfStatus.PreIn;
                        break;
                    case "O":
                        temp.ShelfStatus = shelfStatus.PreOut;
                        break;
                    case "A":
                        temp.ShelfStatus = shelfStatus.Alternate;
                        break;
                    default:
                        break;
                }
                result.ShelfInfo.Add(temp);
                #endregion
            }
            return Task.FromResult(result);
        }
        public override Task<shelf> getShelfInfo(shelf_id id, ServerCallContext context)
        {
            ShelfDef shelfDef;
            shelf result = new shelf();
            try
            {
                shelfDef = app.ShelfDefBLL.LoadShelf().Find(shelf => shelf.ShelfID == id.ID);
                result.BoxId = "";
                result.Enable = (shelfDef.Enable == "Enable");
                result.ShelfId = shelfDef.ShelfID;
                result.ZoneId = shelfDef.ZoneID;
                result.AdrId = shelfDef.ADR_ID;
                #region shelf status
                switch (shelfDef.ShelfState)
                {
                    case "E":
                        result.ShelfStatus = shelfStatus.Empty;
                        break;
                    case "S":
                        result.ShelfStatus = shelfStatus.Store;
                        break;
                    case "I":
                        result.ShelfStatus = shelfStatus.PreIn;
                        break;
                    case "O":
                        result.ShelfStatus = shelfStatus.PreOut;
                        break;
                    case "A":
                        result.ShelfStatus = shelfStatus.Alternate;
                        break;
                    default:
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                //如果find找不到就會跳例外所以在這邊處理
                result.BoxId = "";
                result.Enable = false;
                result.ShelfId = "";
                result.ZoneId = "";
                result.AdrId = "";
            }
            return Task.FromResult(result); //不管find有找到或找不到跳例外都會回傳
        }
        public override Task<replyAllShelfInfo> getNeedChangeShelf(lastUpdateTime dataTime, ServerCallContext context)
        {
            replyAllShelfInfo result = new replyAllShelfInfo();
            //var allShelfData = app.ShelfDefDao.LoadShelfDef(DBConnection_EF.GetUContext());
            DateTime userLastUpdateTime = DateTime.FromBinary(dataTime.Datetime);//這邊是client告訴我們他最後更新的時間
            string s_user_last_update_time = userLastUpdateTime.ToString(sc.App.SCAppConstants.TimestampFormat_19);
            var allShelfData = app.ShelfDefBLL.loadHasChangeShelfDefByAfterDateTime(s_user_last_update_time);
            if (allShelfData == null || allShelfData.Count == 0)
                return Task.FromResult(result);
            foreach (var shelfData in allShelfData)
            {
                shelf temp = new shelf();
                temp.BoxId = "";
                temp.Enable = (shelfData.Enable == "Y") ? true : false;
                temp.ShelfId = shelfData.ShelfID;
                temp.ZoneId = shelfData.ZoneID;
                temp.AdrId = shelfData.ADR_ID;
                #region shelf status
                switch (shelfData.ShelfState)
                {
                    case "E":
                        temp.ShelfStatus = shelfStatus.Empty;
                        break;
                    case "S":
                        temp.ShelfStatus = shelfStatus.Store;
                        break;
                    case "I":
                        temp.ShelfStatus = shelfStatus.PreIn;
                        break;
                    case "O":
                        temp.ShelfStatus = shelfStatus.PreOut;
                        break;
                    case "A":
                        temp.ShelfStatus = shelfStatus.Alternate;
                        break;
                    default:
                        break;
                }
                #endregion
                result.ShelfInfo.Add(temp);
            }
            return Task.FromResult(result);
        }
    }
}
