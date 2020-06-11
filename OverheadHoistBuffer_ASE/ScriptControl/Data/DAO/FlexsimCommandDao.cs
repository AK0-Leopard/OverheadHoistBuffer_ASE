using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class FlexsimCommandDao
    {
        //const string connectionString =@"metadata=res://*/OHTCContext.csdl|res://*/OHTCContext.ssdl|res://*/OHTCContext.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=db.ohxc.mirle.com.tw;initial catalog=OHSC_Dev;user id=sa;password=p@ssw0rd;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
        const string connectionString =
                       "Data Source=flixsim.ohxc.mirle.com.tw;" +
                       "Initial Catalog=flexsim;" +
                       "User id=sa;" +
                       "Password=p@ssw0rd;";
        //const string connectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=D:\Temp\TestData.mdf;Integrated Security=True;User Instance=True";
        SqlConnection connection = new SqlConnection(connectionString);
        public void setCommandToFlexsimDB(ACMD_OHTC cmd)
        {
            //switch (cmd.CMD_TPYE)
            //{
            //    case E_CMD_TYPE.Move:
            //    case E_CMD_TYPE.Move_Park:
            //    case E_CMD_TYPE.Move_MTPort:
            //        setCommandToFlexsimDB(cmd.VH_ID, cmd.DESTINATION, "0",
            //                                         cmd.DESTINATION, "0",
            //                                         cmd.CARRIER_ID);
            //        break;
            //    default:
            //        setCommandToFlexsimDB(cmd.VH_ID, cmd.SOURCE, "1",
            //                                         cmd.DESTINATION, "1",
            //                                         cmd.CARRIER_ID);
            //        break;
            //}
        }
        public void setCommandToFlexsimDB(string car_name, string source, string load_witch,
            string dest, string unload_switch, string item, string done = "0")
        {
            lock (connection)
            {
                //try
                //{
                //    string commandString = $@"INSERT INTO [task]([car_name],[source],[load_switch],[dest],[unload_switch],[item],[done])VALUES
                //       ('{car_name.Trim()}',{source.Trim()},{load_witch.Trim()},{dest.Trim()},{unload_switch.Trim()},'{item.Trim()}',{done.Trim()})";
                //    SqlCommand command = new SqlCommand(commandString, connection);
                //    connection.Open();
                //    command.ExecuteNonQuery();
                //}
                //catch (Exception ex)
                //{
                //    NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception");
                //}
                //finally
                //{
                //    connection.Close();
                //}
            }
        }
        public void setVhPositionToFlexsimDB(string vh_id, string station_id)
        {
            lock (connection)
            {
                //try
                //{
                //    //string commandString = $@"UPDATE [car_initial_position] SET [station] = {station_id.Trim()} WHERE [car] = '{vh_id.Trim()}'";
                //    //SqlCommand command = new SqlCommand(commandString, connection);
                //    //connection.Open();
                //    //command.ExecuteNonQuery();
                //}
                //catch (Exception ex)
                //{
                //    NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception");
                //}
                //finally
                //{
                //    connection.Close();
                //}
            }
        }

        public void setVhStatusToFlexsimDB(string vh_id, string current_adr_id,double sec_dis, ProtocolFormat.OHTMessage.EventType event_type,
                                           string cst_id, ProtocolFormat.OHTMessage.VHModeStatus mode_status, ProtocolFormat.OHTMessage.VHActionStatus action_status,
                                           VhStopSingle obs_pause, VhStopSingle block_pause, VhStopSingle pause, VhStopSingle hid_pause, VhStopSingle error, VhStopSingle earth_quake_pause,
                                           VhStopSingle safety_pause)
        {
            lock (connection)
            {
                //try
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.Append("UPDATE AVEHICLEINFO").Append(" ");
                //    sb.Append("SET").Append(" ");
                //    sb.Append($"CUR_ADR_ID = '{current_adr_id.Trim()}'").Append(",");
                //    sb.Append($"ACC_SEC_DIST = {sec_dis}").Append(",");
                //    sb.Append($"CST_ID = '{cst_id?.Trim()}'").Append(",");
                //    sb.Append($"MODE_STATUS = {(int)mode_status}").Append(",");
                //    sb.Append($"ACT_STATUS = {(int)action_status}").Append(",");
                //    sb.Append($"OBS_PAUSE = {(int)obs_pause}").Append(",");
                //    sb.Append($"BLOCK_PAUSE = {(int)block_pause}").Append(",");
                //    sb.Append($"PAUSE = {(int)pause}").Append(",");
                //    sb.Append($"HID_PAUSE = {(int)hid_pause}").Append(",");
                //    sb.Append($"ERROR = {(int)error}").Append(",");
                //    sb.Append($"EARTHQUAKE_PAUSE = {(int)earth_quake_pause}").Append(",");
                //    sb.Append($"SAFETY_DOOR_PAUSE = {(int)safety_pause}").Append(" ");
                //    sb.Append($"WHERE [ID] = '{vh_id.Trim()}'");
                    

                //    //string commandString = $@"UPDATE AVEHICLEINFO SET CUR_ADR_ID = {current_adr_id.Trim()} ACC_SEC_DIST = {sec_dis} WHERE [ID] = '{vh_id.Trim()}'";
                //    SqlCommand command = new SqlCommand(sb.ToString(), connection);
                //    connection.Open();
                //    command.ExecuteNonQuery();
                //}
                //catch (Exception ex)
                //{
                //    NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception");
                //}
                //finally
                //{
                //    connection.Close();
                //}
            }
        }
        public void setVhEventTypeToFlexsimDB(string vh_id,  ProtocolFormat.OHTMessage.EventType event_type)
        {
            lock (connection)
            {
                //try
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.Append("UPDATE AVEHICLEINFO").Append(" ");
                //    sb.Append("SET").Append(" ");
                //    sb.Append($"EVENT_TYPE = {(int)event_type}").Append(" ");
                //    sb.Append($"WHERE [ID] = '{vh_id.Trim()}'");


                //    //string commandString = $@"UPDATE AVEHICLEINFO SET CUR_ADR_ID = {current_adr_id.Trim()} ACC_SEC_DIST = {sec_dis} WHERE [ID] = '{vh_id.Trim()}'";
                //    SqlCommand command = new SqlCommand(sb.ToString(), connection);
                //    connection.Open();
                //    command.ExecuteNonQuery();
                //}
                //catch (Exception ex)
                //{
                //    NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception");
                //}
                //finally
                //{
                //    connection.Close();
                //}
            }
        }

    }
}
