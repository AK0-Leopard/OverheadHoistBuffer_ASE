
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/17/2020 21:45:11
-- Generated from EDMX file: D:\Working_Projects\OHTC&MCS\TFS_SourceCode\OverheadHoistBuffer_ASE\OverheadHoistBuffer_ASE\ScriptControl\OHTCContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [OHBC_ASE_K21_LOOP];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AADDRESS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AADDRESS];
GO
IF OBJECT_ID(N'[dbo].[AADDRESS_DATA]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AADDRESS_DATA];
GO
IF OBJECT_ID(N'[dbo].[ABASEDATA_VER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ABASEDATA_VER];
GO
IF OBJECT_ID(N'[dbo].[ABLOCKZONEDETAIL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ABLOCKZONEDETAIL];
GO
IF OBJECT_ID(N'[dbo].[ABLOCKZONEMASTER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ABLOCKZONEMASTER];
GO
IF OBJECT_ID(N'[dbo].[ABUFFER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ABUFFER];
GO
IF OBJECT_ID(N'[dbo].[ACASSETTE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACASSETTE];
GO
IF OBJECT_ID(N'[dbo].[ACEID]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACEID];
GO
IF OBJECT_ID(N'[dbo].[ACMD_MCS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACMD_MCS];
GO
IF OBJECT_ID(N'[dbo].[ACMD_OHTC]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACMD_OHTC];
GO
IF OBJECT_ID(N'[dbo].[ACMD_OHTC_DETAIL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACMD_OHTC_DETAIL];
GO
IF OBJECT_ID(N'[dbo].[ACRATE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACRATE];
GO
IF OBJECT_ID(N'[dbo].[ACYCLEZONEDETAIL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACYCLEZONEDETAIL];
GO
IF OBJECT_ID(N'[dbo].[ACYCLEZONEMASTER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACYCLEZONEMASTER];
GO
IF OBJECT_ID(N'[dbo].[ACYCLEZONETYPE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACYCLEZONETYPE];
GO
IF OBJECT_ID(N'[dbo].[AECDATAMAP]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AECDATAMAP];
GO
IF OBJECT_ID(N'[dbo].[AEQPT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AEQPT];
GO
IF OBJECT_ID(N'[dbo].[AEVENTRPTCOND]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AEVENTRPTCOND];
GO
IF OBJECT_ID(N'[dbo].[AFLOW_REL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AFLOW_REL];
GO
IF OBJECT_ID(N'[dbo].[AGROUPRAILS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AGROUPRAILS];
GO
IF OBJECT_ID(N'[dbo].[AHIDZONEDETAIL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AHIDZONEDETAIL];
GO
IF OBJECT_ID(N'[dbo].[AHIDZONEMASTER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AHIDZONEMASTER];
GO
IF OBJECT_ID(N'[dbo].[AHIDZONEQUEUE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AHIDZONEQUEUE];
GO
IF OBJECT_ID(N'[dbo].[ALARM]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ALARM];
GO
IF OBJECT_ID(N'[dbo].[ALARMRPTCOND]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ALARMRPTCOND];
GO
IF OBJECT_ID(N'[dbo].[ALINE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ALINE];
GO
IF OBJECT_ID(N'[dbo].[ALOT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ALOT];
GO
IF OBJECT_ID(N'[dbo].[AMAIN_VER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AMAIN_VER];
GO
IF OBJECT_ID(N'[dbo].[AMCSREPORTQUEUE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AMCSREPORTQUEUE];
GO
IF OBJECT_ID(N'[dbo].[ANETWORKQUALITY]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ANETWORKQUALITY];
GO
IF OBJECT_ID(N'[dbo].[ANODE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ANODE];
GO
IF OBJECT_ID(N'[dbo].[APARKZONEDETAIL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APARKZONEDETAIL];
GO
IF OBJECT_ID(N'[dbo].[APARKZONEMASTER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APARKZONEMASTER];
GO
IF OBJECT_ID(N'[dbo].[APARKZONETYPE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APARKZONETYPE];
GO
IF OBJECT_ID(N'[dbo].[APOINT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APOINT];
GO
IF OBJECT_ID(N'[dbo].[APORT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APORT];
GO
IF OBJECT_ID(N'[dbo].[APORT_POSITION_TEACHING_DATA]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APORT_POSITION_TEACHING_DATA];
GO
IF OBJECT_ID(N'[dbo].[APORTICON]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APORTICON];
GO
IF OBJECT_ID(N'[dbo].[APORTSTATION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APORTSTATION];
GO
IF OBJECT_ID(N'[dbo].[ARAIL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ARAIL];
GO
IF OBJECT_ID(N'[dbo].[ARPTID]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ARPTID];
GO
IF OBJECT_ID(N'[dbo].[ASECTION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASECTION];
GO
IF OBJECT_ID(N'[dbo].[ASECTION_CONTROL_100]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASECTION_CONTROL_100];
GO
IF OBJECT_ID(N'[dbo].[ASEGMENT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASEGMENT];
GO
IF OBJECT_ID(N'[dbo].[ASEQUENCE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASEQUENCE];
GO
IF OBJECT_ID(N'[dbo].[ASHEET]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASHEET];
GO
IF OBJECT_ID(N'[dbo].[ASUB_VER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASUB_VER];
GO
IF OBJECT_ID(N'[dbo].[ASYSEXCUTEQUALITY]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASYSEXCUTEQUALITY];
GO
IF OBJECT_ID(N'[dbo].[ATRACEITEM]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ATRACEITEM];
GO
IF OBJECT_ID(N'[dbo].[ATRACESET]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ATRACESET];
GO
IF OBJECT_ID(N'[dbo].[AUNIT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AUNIT];
GO
IF OBJECT_ID(N'[dbo].[AVEHICLE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AVEHICLE];
GO
IF OBJECT_ID(N'[dbo].[AVEHICLE_CONTROL_100]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AVEHICLE_CONTROL_100];
GO
IF OBJECT_ID(N'[dbo].[AVIDINFO]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AVIDINFO];
GO
IF OBJECT_ID(N'[dbo].[AZONE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AZONE];
GO
IF OBJECT_ID(N'[dbo].[BCSTAT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BCSTAT];
GO
IF OBJECT_ID(N'[dbo].[BLOCKZONEQUEUE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BLOCKZONEQUEUE];
GO
IF OBJECT_ID(N'[dbo].[CassetteData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CassetteData];
GO
IF OBJECT_ID(N'[dbo].[CONTROL_DATA]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CONTROL_DATA];
GO
IF OBJECT_ID(N'[dbo].[HASHEET]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HASHEET];
GO
IF OBJECT_ID(N'[dbo].[HOPERATION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HOPERATION];
GO
IF OBJECT_ID(N'[dbo].[PortDef]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PortDef];
GO
IF OBJECT_ID(N'[dbo].[SCALE_BASE_DATA]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SCALE_BASE_DATA];
GO
IF OBJECT_ID(N'[dbo].[ShelfDef]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShelfDef];
GO
IF OBJECT_ID(N'[dbo].[UASFNC]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UASFNC];
GO
IF OBJECT_ID(N'[dbo].[UASUFNC]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UASUFNC];
GO
IF OBJECT_ID(N'[dbo].[UASUSR]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UASUSR];
GO
IF OBJECT_ID(N'[dbo].[UASUSRGRP]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UASUSRGRP];
GO
IF OBJECT_ID(N'[dbo].[ZoneDef]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ZoneDef];
GO
IF OBJECT_ID(N'[OHTC_DevModelStoreContainer].[ALARMMAP]', 'U') IS NOT NULL
    DROP TABLE [OHTC_DevModelStoreContainer].[ALARMMAP];
GO
IF OBJECT_ID(N'[OHTC_DevModelStoreContainer].[VACMD_MCS]', 'U') IS NOT NULL
    DROP TABLE [OHTC_DevModelStoreContainer].[VACMD_MCS];
GO
IF OBJECT_ID(N'[OHTC_DevModelStoreContainer].[VSECTION_100]', 'U') IS NOT NULL
    DROP TABLE [OHTC_DevModelStoreContainer].[VSECTION_100];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ABASEDATA_VER'
CREATE TABLE [dbo].[ABASEDATA_VER] (
    [VER_TYPE] int  NOT NULL,
    [SUB_VER] char(4)  NOT NULL,
    [REL_STAUS] int  NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL,
    [MEMO] nchar(500)  NULL
);
GO

-- Creating table 'ABLOCKZONEDETAIL'
CREATE TABLE [dbo].[ABLOCKZONEDETAIL] (
    [ENTRY_SEC_ID] nchar(5)  NOT NULL,
    [SEC_ID] nchar(5)  NOT NULL
);
GO

-- Creating table 'AGROUPRAILS'
CREATE TABLE [dbo].[AGROUPRAILS] (
    [SECTION_ID] char(5)  NOT NULL,
    [RAIL_ID] char(5)  NOT NULL,
    [RAIL_NO] int  NULL,
    [DIR] int  NOT NULL
);
GO

-- Creating table 'AMAIN_VER'
CREATE TABLE [dbo].[AMAIN_VER] (
    [VER_YEAR] char(4)  NULL,
    [VER_MON] char(2)  NULL,
    [VER_DAY] char(2)  NULL,
    [VER_HOUR] char(2)  NULL,
    [VER_MIN] char(2)  NULL,
    [MAIN_VER] char(4)  NOT NULL,
    [VER_STAUS] int  NULL,
    [REL_STAUS] int  NULL,
    [MEMO] nchar(500)  NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL
);
GO

-- Creating table 'APOINT'
CREATE TABLE [dbo].[APOINT] (
    [POINT_ID] char(5)  NOT NULL,
    [ADR_ID] char(5)  NULL,
    [RAIL_ID] char(5)  NULL,
    [POINTTYPE] int  NOT NULL,
    [LOCATIONX] int  NOT NULL,
    [LOCATIONY] int  NOT NULL,
    [HEIGHT] float  NOT NULL,
    [WIDTH] float  NOT NULL,
    [COLOR] char(10)  NOT NULL
);
GO

-- Creating table 'ARAIL'
CREATE TABLE [dbo].[ARAIL] (
    [RAIL_ID] char(5)  NOT NULL,
    [RAILTYPE] int  NOT NULL,
    [LOCATIONX] int  NOT NULL,
    [LOCATIONY] int  NOT NULL,
    [WIDTH] float  NOT NULL,
    [LENGTH] float  NOT NULL,
    [COLOR] char(10)  NULL
);
GO

-- Creating table 'ASEGMENT'
CREATE TABLE [dbo].[ASEGMENT] (
    [SEG_NUM] char(10)  NOT NULL,
    [STATUS] int  NOT NULL,
    [SEG_TYPE] int  NOT NULL,
    [SPECIAL_MARK] int  NULL,
    [RESERVE_FIELD] char(40)  NULL,
    [NOTE] nchar(40)  NULL,
    [DIR] int  NOT NULL,
    [PRE_DISABLE_FLAG] bit  NOT NULL,
    [PRE_DISABLE_TIME] datetime  NULL,
    [DISABLE_TIME] datetime  NULL,
    [DISABLE_FLAG_USER] bit  NOT NULL,
    [DISABLE_FLAG_SAFETY] bit  NOT NULL,
    [DISABLE_FLAG_HID] bit  NOT NULL,
    [DISABLE_FLAG_SYSTEM] bit  NOT NULL
);
GO

-- Creating table 'ASUB_VER'
CREATE TABLE [dbo].[ASUB_VER] (
    [MAIN_VER] char(4)  NOT NULL,
    [VER_TYPE] int  NOT NULL,
    [SUB_VER] char(4)  NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL
);
GO

-- Creating table 'BLOCKZONEQUEUE'
CREATE TABLE [dbo].[BLOCKZONEQUEUE] (
    [ENTRY_SEC_ID] nchar(5)  NOT NULL,
    [CAR_ID] nchar(10)  NOT NULL,
    [REQ_TIME] datetime  NOT NULL,
    [BLOCK_TIME] datetime  NULL,
    [THROU_TIME] datetime  NULL,
    [RELEASE_TIME] datetime  NULL,
    [STATUS] char(1)  NULL
);
GO

-- Creating table 'ABLOCKZONEMASTER'
CREATE TABLE [dbo].[ABLOCKZONEMASTER] (
    [ENTRY_SEC_ID] char(5)  NOT NULL,
    [BLOCK_ZONE_TYPE] int  NOT NULL,
    [LEAVE_ADR_ID_1] char(5)  NOT NULL,
    [LEAVE_ADR_ID_2] char(5)  NOT NULL
);
GO

-- Creating table 'ASEQUENCE'
CREATE TABLE [dbo].[ASEQUENCE] (
    [SEQ_NAME] char(15)  NOT NULL,
    [NXT_VAL] bigint  NOT NULL
);
GO

-- Creating table 'ACYCLEZONEDETAIL'
CREATE TABLE [dbo].[ACYCLEZONEDETAIL] (
    [CYCLE_ZONE_ID] char(10)  NOT NULL,
    [SEC_ID] char(10)  NOT NULL,
    [SEC_ORDER] int  NOT NULL
);
GO

-- Creating table 'ACYCLEZONEMASTER'
CREATE TABLE [dbo].[ACYCLEZONEMASTER] (
    [CYCLE_TYPE_ID] char(10)  NOT NULL,
    [CYCLE_ZONE_ID] char(10)  NOT NULL,
    [ENTRY_ADR_ID] char(5)  NOT NULL,
    [TOTAL_BORDER] int  NOT NULL,
    [VEHICLE_TYPE] int  NOT NULL
);
GO

-- Creating table 'ACYCLEZONETYPE'
CREATE TABLE [dbo].[ACYCLEZONETYPE] (
    [CYCLE_TYPE_ID] char(10)  NOT NULL,
    [PROD_ID] char(10)  NULL,
    [TOTAL_BORDER] int  NOT NULL,
    [IS_DEFAULT] int  NOT NULL
);
GO

-- Creating table 'AADDRESS'
CREATE TABLE [dbo].[AADDRESS] (
    [ADR_ID] char(5)  NOT NULL,
    [ADRTYPE] int  NOT NULL,
    [SEC_ID] char(5)  NULL,
    [DISTANCE] int  NULL,
    [CS_NUMBER] char(10)  NULL,
    [T_PULSE] char(10)  NULL,
    [P_LOCATION] char(10)  NOT NULL,
    [IS_DISPLAY] int  NOT NULL,
    [ZOOM_LV] int  NOT NULL,
    [BAR_CODE] char(10)  NULL,
    [OFFSET_X] int  NOT NULL,
    [OFFSET_Y] int  NOT NULL,
    [OFFSET_Z] int  NOT NULL,
    [OFFSET_T] int  NOT NULL,
    [PIO_ID] char(6)  NULL,
    [PIO_CHN] int  NOT NULL,
    [PORT1_ID] char(10)  NULL,
    [P1_LD_VH_TYPE] int  NOT NULL,
    [P1_ULD_VH_TYPE] int  NOT NULL,
    [PORT2_ID] char(10)  NULL,
    [P2_LD_VH_TYPE] int  NOT NULL,
    [P2_ULD_VH_TYPE] int  NOT NULL
);
GO

-- Creating table 'ACMD_OHTC'
CREATE TABLE [dbo].[ACMD_OHTC] (
    [CMD_ID] char(64)  NOT NULL,
    [VH_ID] char(32)  NOT NULL,
    [CARRIER_ID] char(64)  NULL,
    [CMD_ID_MCS] char(64)  NULL,
    [CMD_TPYE] int  NOT NULL,
    [SOURCE] char(64)  NULL,
    [DESTINATION] char(64)  NULL,
    [PRIORITY] int  NOT NULL,
    [CMD_START_TIME] datetime  NULL,
    [CMD_END_TIME] datetime  NULL,
    [CMD_STAUS] int  NOT NULL,
    [CMD_PROGRESS] int  NOT NULL,
    [INTERRUPTED_REASON] int  NULL,
    [ESTIMATED_TIME] int  NOT NULL,
    [ESTIMATED_EXCESS_TIME] int  NOT NULL,
    [REAL_CMP_TIME] int  NULL,
    [SOURCE_ADR] char(64)  NULL,
    [DESTINATION_ADR] char(64)  NULL,
    [BOX_ID] char(64)  NULL,
    [LOT_ID] char(64)  NULL
);
GO

-- Creating table 'ACEID'
CREATE TABLE [dbo].[ACEID] (
    [CEID] char(3)  NOT NULL,
    [RPTID] char(3)  NOT NULL,
    [ORDER_NUM] int  NOT NULL,
    [NAME] char(20)  NULL,
    [UPD_TIME] datetime  NULL
);
GO

-- Creating table 'ARPTID'
CREATE TABLE [dbo].[ARPTID] (
    [RPTID] char(3)  NOT NULL,
    [VID] char(3)  NOT NULL,
    [ORDER_NUM] int  NOT NULL,
    [NAME] char(20)  NULL,
    [UPD_TIME] datetime  NULL
);
GO

-- Creating table 'AVIDINFO'
CREATE TABLE [dbo].[AVIDINFO] (
    [EQ_ID] char(32)  NOT NULL,
    [MCS_CARRIER_ID] char(64)  NULL,
    [CARRIER_ID] char(64)  NULL,
    [CARRIER_LOC] char(64)  NULL,
    [CARRIER_INSTALLED_TIME] datetime  NULL,
    [COMMAND_ID] char(64)  NULL,
    [SOURCEPORT] char(64)  NULL,
    [DESTPORT] char(64)  NULL,
    [PRIORITY] int  NOT NULL,
    [RESULT_CODE] int  NOT NULL,
    [VEHICLE_STATE] int  NOT NULL,
    [COMMAND_TYPE] char(64)  NULL,
    [ALARM_ID] char(64)  NULL,
    [ALARM_TEXT] char(64)  NULL,
    [UNIT_ID] char(64)  NULL,
    [PORT_ID] char(64)  NULL,
    [REPLACE] int  NOT NULL
);
GO

-- Creating table 'ANETWORKQUALITY'
CREATE TABLE [dbo].[ANETWORKQUALITY] (
    [VEICLE_ID] char(32)  NOT NULL,
    [ADR_ID] char(5)  NOT NULL,
    [SEC_ID] char(5)  NOT NULL,
    [ACC_SEC_DIST] int  NOT NULL,
    [UPD_TIME] datetime  NOT NULL,
    [PING_TIME] bigint  NOT NULL
);
GO

-- Creating table 'APORTICON'
CREATE TABLE [dbo].[APORTICON] (
    [PORT_ID] char(10)  NOT NULL,
    [ADR_ID] char(5)  NOT NULL,
    [RAIL_ID] char(5)  NOT NULL,
    [ADRTYPE] int  NOT NULL,
    [LOCATIONX] int  NOT NULL,
    [LOCATIONY] int  NOT NULL,
    [HEIGHT] int  NOT NULL,
    [WIDTH] int  NOT NULL,
    [COLOR] char(10)  NOT NULL
);
GO

-- Creating table 'BCSTAT'
CREATE TABLE [dbo].[BCSTAT] (
    [BC_ID] char(10)  NOT NULL,
    [CLOSE_MODE] char(1)  NOT NULL,
    [RUN_TIMESTAMP] char(16)  NULL
);
GO

-- Creating table 'AEQPT'
CREATE TABLE [dbo].[AEQPT] (
    [EQPT_ID] char(15)  NOT NULL,
    [NODE_ID] char(15)  NOT NULL,
    [CIM_MODE] char(1)  NOT NULL,
    [OPER_MODE] char(1)  NOT NULL,
    [INLINE_MODE] char(1)  NOT NULL,
    [EQPT_STAT] char(1)  NOT NULL,
    [EQPT_PROC_STAT] char(1)  NOT NULL,
    [CR_RECIPE] char(33)  NULL,
    [MAX_SHT_CNT] int  NULL,
    [MIN_SHT_CNT] int  NULL,
    [ALARM_STAT] char(1)  NULL,
    [WARN_STAT] char(1)  NULL
);
GO

-- Creating table 'ALINE'
CREATE TABLE [dbo].[ALINE] (
    [LINE_ID] char(15)  NOT NULL,
    [HOST_MODE] int  NOT NULL,
    [LINE_STAT] int  NOT NULL
);
GO

-- Creating table 'AZONE'
CREATE TABLE [dbo].[AZONE] (
    [ZONE_ID] char(15)  NOT NULL,
    [LINE_ID] char(15)  NULL,
    [LOT_ID] char(20)  NULL
);
GO

-- Creating table 'ANODE'
CREATE TABLE [dbo].[ANODE] (
    [NODE_ID] char(15)  NOT NULL,
    [ZONE_ID] char(15)  NOT NULL
);
GO

-- Creating table 'AUNIT'
CREATE TABLE [dbo].[AUNIT] (
    [UNIT_ID] char(15)  NOT NULL,
    [UNIT_NUM] int  NOT NULL,
    [EQPT_ID] char(15)  NOT NULL,
    [UNIT_CATE] char(1)  NOT NULL,
    [EQPT_TYPE] char(1)  NOT NULL,
    [CAPACITY] int  NOT NULL,
    [UNIT_STAT] int  NOT NULL
);
GO

-- Creating table 'ABUFFER'
CREATE TABLE [dbo].[ABUFFER] (
    [BUFF_ID] char(15)  NOT NULL,
    [UNIT_NUM] int  NOT NULL,
    [EQPT_ID] char(15)  NOT NULL,
    [CAPACITY] int  NOT NULL
);
GO

-- Creating table 'ACASSETTE'
CREATE TABLE [dbo].[ACASSETTE] (
    [CST_ID] char(20)  NOT NULL,
    [CST_STAT] int  NOT NULL,
    [SHT_CNT] int  NOT NULL,
    [PORT_ID] char(15)  NOT NULL,
    [CST_CODE] char(1)  NOT NULL,
    [LOT_ID] char(20)  NULL,
    [SLOT_MAP] char(20)  NOT NULL,
    [SLOT_SEL] char(20)  NULL,
    [END_STAT] char(1)  NOT NULL,
    [CRATE_ID] char(20)  NULL,
    [CSTLOGON_TIME] datetime  NULL,
    [CSTLOGOff_TIME] datetime  NULL,
    [ISLOTSTART] char(1)  NOT NULL,
    [CSTPROCSTART_TIME] datetime  NULL,
    [CSTPROCEND_TIME] datetime  NULL,
    [ROUNTINMODE] char(1)  NULL
);
GO

-- Creating table 'ACRATE'
CREATE TABLE [dbo].[ACRATE] (
    [CRATE_ID] char(20)  NOT NULL,
    [MAKER] char(20)  NULL
);
GO

-- Creating table 'AECDATAMAP'
CREATE TABLE [dbo].[AECDATAMAP] (
    [ECID] char(4)  NOT NULL,
    [EQPT_REAL_ID] char(15)  NULL,
    [ECNAME] char(40)  NULL,
    [ECMIN] char(10)  NULL,
    [ECMAX] char(10)  NULL,
    [ECV] char(10)  NULL
);
GO

-- Creating table 'AEVENTRPTCOND'
CREATE TABLE [dbo].[AEVENTRPTCOND] (
    [CEID] char(3)  NOT NULL,
    [ENABLE_FLG] char(1)  NOT NULL
);
GO

-- Creating table 'AFLOW_REL'
CREATE TABLE [dbo].[AFLOW_REL] (
    [UPSTREAM_ID] char(10)  NOT NULL,
    [DOWNSTREAM_ID] char(10)  NOT NULL,
    [FR_ID] char(10)  NOT NULL,
    [REL_TYPE] char(1)  NOT NULL
);
GO

-- Creating table 'UASFNC'
CREATE TABLE [dbo].[UASFNC] (
    [FUNC_CODE] char(60)  NOT NULL,
    [FUNC_NAME] char(80)  NULL
);
GO

-- Creating table 'ALOT'
CREATE TABLE [dbo].[ALOT] (
    [LOT_ID] char(20)  NOT NULL,
    [SHT_TYP] char(1)  NULL,
    [CST_OPER_MODE] char(1)  NOT NULL,
    [DUMMY_TYPE] char(1)  NOT NULL,
    [LOT_JUDGE] char(1)  NULL,
    [WORK_ORDER] char(30)  NULL,
    [PROC_STAT] char(1)  NULL,
    [LOT_START_PROC_TIME] char(16)  NULL,
    [LOT_END_PROC_TIME] char(16)  NULL,
    [PROC_SHT_CNT] int  NULL
);
GO

-- Creating table 'HOPERATION'
CREATE TABLE [dbo].[HOPERATION] (
    [SEQ_NO] nvarchar(255)  NOT NULL,
    [T_STAMP] char(19)  NULL,
    [USER_ID] char(20)  NULL,
    [FORM_NAME] char(30)  NULL,
    [ACTION] varchar(max)  NULL
);
GO

-- Creating table 'ASHEET'
CREATE TABLE [dbo].[ASHEET] (
    [SHT_ID] char(20)  NOT NULL,
    [JOB_NO] char(5)  NOT NULL,
    [LOT_ID] char(20)  NOT NULL,
    [PROD_ID] char(20)  NULL,
    [SHT_STAT] char(1)  NULL,
    [SOURCE_CST_ID] char(20)  NULL,
    [SOURCE_SLOT_NO] int  NOT NULL,
    [SOURCE_PORT_NO] int  NOT NULL,
    [TARGET_CST_ID] char(20)  NULL,
    [TARGET_SLOT_NO] int  NOT NULL,
    [TARGET_PORT_NO] int  NOT NULL,
    [PROC_FLAG] char(8)  NULL,
    [SHT_JUDGE] char(1)  NULL,
    [SHT_GRADE] char(1)  NULL,
    [PROC_INFO] char(8)  NULL,
    [PPID] char(40)  NULL,
    [CRATE_ID] char(20)  NULL,
    [TAKE_OUT_STAT] char(1)  NULL,
    [NODE_ID] char(15)  NULL,
    [CST_ID] char(20)  NULL,
    [SLOT_NO] int  NOT NULL,
    [TAKE_OUT_TIME] char(16)  NULL,
    [SCRAP_CODE] char(5)  NULL,
    [REASON_CODE] char(5)  NULL
);
GO

-- Creating table 'HASHEET'
CREATE TABLE [dbo].[HASHEET] (
    [SEQ_NO] nvarchar(255)  NOT NULL,
    [T_STAMP] char(19)  NULL,
    [SHT_ID] char(20)  NULL,
    [JOB_NO] char(5)  NOT NULL,
    [LOT_ID] char(20)  NOT NULL,
    [PROD_ID] char(20)  NULL,
    [SHT_STAT] char(1)  NULL,
    [SOURCE_CST_ID] char(20)  NULL,
    [SOURCE_SLOT_NO] int  NOT NULL,
    [SOURCE_PORT_NO] int  NOT NULL,
    [TARGET_CST_ID] char(20)  NULL,
    [TARGET_SLOT_NO] int  NOT NULL,
    [TARGET_PORT_NO] int  NOT NULL,
    [PROC_FLAG] char(8)  NULL,
    [SHT_JUDGE] char(1)  NULL,
    [SHT_GRADE] char(1)  NULL,
    [PROC_INFO] char(8)  NULL,
    [PPID] char(40)  NULL,
    [CRATE_ID] char(20)  NULL,
    [TAKE_OUT_STAT] char(1)  NULL,
    [NODE_ID] char(15)  NULL,
    [CST_ID] char(20)  NULL,
    [SLOT_NO] int  NOT NULL,
    [TAKE_OUT_TIME] char(16)  NULL,
    [SCRAP_CODE] char(5)  NULL,
    [REASON_CODE] char(5)  NULL
);
GO

-- Creating table 'ATRACEITEM'
CREATE TABLE [dbo].[ATRACEITEM] (
    [TRACE_ID] char(2)  NOT NULL,
    [SVID] char(5)  NOT NULL
);
GO

-- Creating table 'ATRACESET'
CREATE TABLE [dbo].[ATRACESET] (
    [TRACE_ID] char(2)  NOT NULL,
    [SMP_PERIOD] char(6)  NULL,
    [SMP_PERIOD_SEC] bigint  NOT NULL,
    [TOTAL_SMP_CNT] int  NOT NULL,
    [SMP_CNT] int  NOT NULL,
    [NX_SMP_TIME] datetime  NOT NULL,
    [SMP_TIME] datetime  NOT NULL
);
GO

-- Creating table 'UASUSR'
CREATE TABLE [dbo].[UASUSR] (
    [USER_ID] char(20)  NOT NULL,
    [PASSWD] varchar(max)  NULL,
    [BADGE_NUMBER] char(80)  NULL,
    [USER_NAME] char(30)  NULL,
    [DISABLE_FLG] char(1)  NULL,
    [POWER_USER_FLG] char(1)  NULL,
    [ADMIN_FLG] char(1)  NULL,
    [USER_GRP] char(20)  NULL,
    [DEPARTMENT] char(20)  NULL
);
GO

-- Creating table 'UASUSRGRP'
CREATE TABLE [dbo].[UASUSRGRP] (
    [USER_GRP] char(20)  NOT NULL
);
GO

-- Creating table 'UASUFNC'
CREATE TABLE [dbo].[UASUFNC] (
    [USER_GRP] char(20)  NOT NULL,
    [FUNC_CODE] char(60)  NOT NULL
);
GO

-- Creating table 'APORTSTATION'
CREATE TABLE [dbo].[APORTSTATION] (
    [PORT_ID] char(15)  NOT NULL,
    [ADRTYPE] int  NOT NULL,
    [ADR_ID] char(5)  NULL,
    [LD_VH_TYPE] int  NOT NULL,
    [ULD_VH_TYPE] int  NOT NULL,
    [PRIORITY] int  NOT NULL,
    [PORT_TYPE] int  NOT NULL,
    [PORT_STATUS] int  NOT NULL,
    [PORT_DIR] int  NOT NULL,
    [PORT_SERVICE_STATUS] int  NOT NULL
);
GO

-- Creating table 'APORT'
CREATE TABLE [dbo].[APORT] (
    [PORT_ID] char(15)  NOT NULL,
    [UNIT_NUM] int  NOT NULL,
    [PORT_NUM] int  NOT NULL,
    [EQPT_ID] char(15)  NOT NULL,
    [PORT_TYPE] char(1)  NOT NULL,
    [PORT_USE_TYPE] char(2)  NULL,
    [PORT_REAL_TYPE] char(2)  NULL,
    [CAPACITY] int  NOT NULL,
    [PORT_STAT] char(1)  NOT NULL,
    [PORT_ENABLE] char(1)  NOT NULL,
    [TRS_MODE] char(1)  NULL
);
GO

-- Creating table 'AMCSREPORTQUEUE'
CREATE TABLE [dbo].[AMCSREPORTQUEUE] (
    [ID] uniqueidentifier  NOT NULL,
    [SERIALIZED_SXFY] varbinary(max)  NOT NULL,
    [INTER_TIME] datetime  NOT NULL,
    [REPORT_TIME] datetime  NULL,
    [STREAMFUNCTION_NAME] varchar(50)  NOT NULL,
    [STREAMFUNCTION_CEID] char(5)  NULL,
    [MCS_CMD_ID] char(64)  NULL,
    [VEHICLE_ID] char(32)  NULL,
    [PORT_ID] char(64)  NULL
);
GO

-- Creating table 'AADDRESS_DATA'
CREATE TABLE [dbo].[AADDRESS_DATA] (
    [ADR_ID] char(5)  NOT NULL,
    [VEHOCLE_ID] char(5)  NOT NULL,
    [RESOLUTION] int  NOT NULL,
    [LOACTION] int  NOT NULL
);
GO

-- Creating table 'ACMD_OHTC_DETAIL'
CREATE TABLE [dbo].[ACMD_OHTC_DETAIL] (
    [CMD_ID] char(64)  NOT NULL,
    [SEQ_NO] int  NOT NULL,
    [ADD_ID] char(5)  NOT NULL,
    [ADD_ENTRY_TIME] datetime  NULL,
    [SEC_ID] char(10)  NOT NULL,
    [SEC_ENTRY_TIME] datetime  NULL,
    [SEC_LEAVE_TIME] datetime  NULL,
    [LOAD_START_TIME] datetime  NULL,
    [LOAD_END_TIME] datetime  NULL,
    [UNLOAD_START_TIME] datetime  NULL,
    [UNLOAD_END_TIME] datetime  NULL,
    [ESTIMATED_TIME] int  NOT NULL,
    [IS_PASS] bit  NOT NULL,
    [SEG_NUM] char(10)  NOT NULL
);
GO

-- Creating table 'APARKZONEDETAIL'
CREATE TABLE [dbo].[APARKZONEDETAIL] (
    [PARK_ZONE_ID] char(10)  NOT NULL,
    [ADR_ID] char(5)  NOT NULL,
    [PRIO] int  NOT NULL,
    [CAR_ID] char(5)  NULL
);
GO

-- Creating table 'APARKZONEMASTER'
CREATE TABLE [dbo].[APARKZONEMASTER] (
    [PARK_TYPE_ID] char(10)  NOT NULL,
    [PARK_ZONE_ID] char(10)  NOT NULL,
    [VEHICLE_TYPE] int  NOT NULL,
    [ENTRY_ADR_ID] char(5)  NOT NULL,
    [TOTAL_BORDER] int  NOT NULL,
    [LOWER_BORDER] int  NOT NULL,
    [PARK_TYPE] int  NOT NULL,
    [IS_ACTIVE] bit  NOT NULL
);
GO

-- Creating table 'APARKZONETYPE'
CREATE TABLE [dbo].[APARKZONETYPE] (
    [PARK_TYPE_ID] char(10)  NOT NULL,
    [PROD_ID] char(10)  NULL,
    [TOTAL_BORDER] int  NOT NULL,
    [IS_DEFAULT] int  NOT NULL
);
GO

-- Creating table 'APORT_POSITION_TEACHING_DATA'
CREATE TABLE [dbo].[APORT_POSITION_TEACHING_DATA] (
    [PORT_ID] char(10)  NOT NULL,
    [VH_ID] char(10)  NOT NULL,
    [POSITION_OFFSET] int  NOT NULL,
    [RESOLUTION] int  NOT NULL,
    [SUB_VER] char(4)  NOT NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL
);
GO

-- Creating table 'ASECTION'
CREATE TABLE [dbo].[ASECTION] (
    [SEC_ID] char(5)  NOT NULL,
    [SEC_ORDER_NUM] int  NULL,
    [SEG_ORDER_NUM] int  NOT NULL,
    [DIRC_DRIV] int  NOT NULL,
    [DIRC_GUID] int  NOT NULL,
    [AREA_SECSOR] int  NULL,
    [SEG_NUM] char(3)  NULL,
    [FROM_ADR_ID] char(5)  NULL,
    [TO_ADR_ID] char(5)  NULL,
    [SEC_DIS] float  NOT NULL,
    [SEC_SPD] float  NULL,
    [DIS_FROM_ORIGIN] int  NOT NULL,
    [CDOG_1] int  NOT NULL,
    [CHG_SEG_NUM_1] char(3)  NULL,
    [CDOG_2] int  NOT NULL,
    [CHG_SEG_NUM_2] char(3)  NULL,
    [PRE_BLO_REQ] int  NOT NULL,
    [SEC_TYPE] int  NOT NULL,
    [SEC_DIR] int  NOT NULL,
    [PADDING] int  NOT NULL,
    [ENB_CHG_G_AREA] int  NOT NULL,
    [PRE_DIV] int  NOT NULL,
    [PRE_ADD_REPR] int  NOT NULL,
    [OBS_SENSOR] int  NOT NULL,
    [LAST_TECH_TIME] datetime  NULL,
    [SUB_VER] char(4)  NOT NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL,
    [START_BC1] int  NOT NULL,
    [END_BC1] int  NOT NULL,
    [START_BC2] int  NOT NULL,
    [END_BC2] int  NOT NULL,
    [START_BC3] int  NOT NULL,
    [END_BC3] int  NOT NULL,
    [ADR1_CHG_SEC_ID_1] char(5)  NULL,
    [ADR1_CHG_SEC_COST_1] int  NOT NULL,
    [ADR1_CHG_SEC_ID_2] char(5)  NULL,
    [ADR1_CHG_SEC_COST_2] int  NOT NULL,
    [ADR1_CHG_SEC_ID_3] char(5)  NULL,
    [ADR1_CHG_SEC_COST_3] int  NOT NULL,
    [ADR2_CHG_SEC_ID_1] char(5)  NULL,
    [ADR2_CHG_SEC_COST_1] int  NOT NULL,
    [ADR2_CHG_SEC_ID_2] char(5)  NULL,
    [ADR2_CHG_SEC_COST_2] int  NOT NULL,
    [ADR2_CHG_SEC_ID_3] char(5)  NULL,
    [ADR2_CHG_SEC_COST_3] int  NOT NULL,
    [SEC_COST_From2To] int  NOT NULL,
    [SEC_COST_To2From] int  NOT NULL,
    [ISBANEND_From2To] bit  NOT NULL,
    [ISBANEND_To2From] bit  NOT NULL,
    [STATUS] int  NOT NULL,
    [NOTE] nchar(40)  NULL,
    [PRE_DISABLE_FLAG] bit  NOT NULL,
    [PRE_DISABLE_TIME] datetime  NULL,
    [DISABLE_TIME] datetime  NULL,
    [DISABLE_FLAG_USER] bit  NOT NULL,
    [DISABLE_FLAG_SAFETY] bit  NOT NULL,
    [DISABLE_FLAG_CHARGE] bit  NOT NULL,
    [DISABLE_FLAG_SYSTEM] bit  NOT NULL
);
GO

-- Creating table 'ASECTION_CONTROL_100'
CREATE TABLE [dbo].[ASECTION_CONTROL_100] (
    [SEC_ID] char(5)  NOT NULL,
    [CHG_AREA_SECSOR_1] int  NOT NULL,
    [CHG_AREA_SECSOR_2] int  NOT NULL,
    [OBS_SENSOR_F] int  NOT NULL,
    [OBS_SENSOR_R] int  NOT NULL,
    [OBS_SENSOR_L] int  NOT NULL,
    [RANGE_SENSOR_F] int  NOT NULL,
    [IS_ADR_RPT] bit  NOT NULL,
    [CAN_GUIDE_CHG] bit  NOT NULL,
    [HID_CONTROL] bit  NOT NULL,
    [BRANCH_FLAG] bit  NOT NULL,
    [SUB_VER] char(4)  NOT NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL
);
GO

-- Creating table 'AVEHICLE_CONTROL_100'
CREATE TABLE [dbo].[AVEHICLE_CONTROL_100] (
    [VEHICLE_ID] char(32)  NOT NULL,
    [GUIDE_START_STOP_SPEED] int  NOT NULL,
    [GUIDE_MAX_SPD] int  NOT NULL,
    [GUIDE_ACCEL_DECCEL_TIME] int  NOT NULL,
    [GUIDE_S_CURVE_RATE] int  NOT NULL,
    [GUIDE_RUN_SPD] int  NOT NULL,
    [GUIDE_MANUAL_HIGH_SPD] int  NOT NULL,
    [GUIDE_MANUAL_LOW_SPD] int  NOT NULL,
    [GUIDE_LF_LOCK_POSITION] int  NOT NULL,
    [GUIDE_LB_LOCK_POSITION] int  NOT NULL,
    [GUIDE_RF_LOCK_POSITION] int  NOT NULL,
    [GUIDE_RB_LOCK_POSITION] int  NOT NULL,
    [GUIDE_CHG_STABLE_TIME] int  NOT NULL,
    [TRAVEL_RESOLUTION] int  NOT NULL,
    [TRAVEL_START_STOP_SPEED] int  NOT NULL,
    [TRAVEL_MAX_SPD] int  NOT NULL,
    [TRAVEL_ACCEL_DECCEL_TIME] int  NOT NULL,
    [TRAVEL_S_CURVE_RATE] int  NOT NULL,
    [TRAVEL_HOME_DIR] int  NOT NULL,
    [TRAVEL_HOME_SPD] int  NOT NULL,
    [TRAVEL_KEEP_DIS_SPD] int  NOT NULL,
    [TRAVEL_MANUAL_HIGH_SPD] int  NOT NULL,
    [TRAVEL_MANUAL_LOW_SPD] int  NOT NULL,
    [TRAVEL_TEACHING_SPD] int  NOT NULL,
    [TRAVEL_TRAVEL_DIR] int  NOT NULL,
    [TRAVEL_ENCODER_POLARITY] int  NOT NULL,
    [TRAVEL_F_DIR_LIMIT] int  NOT NULL,
    [TRAVEL_R_DIR_LIMIT] int  NOT NULL,
    [TRAVEL_OBS_DETECT_LONG] int  NOT NULL,
    [TRAVEL_OBS_DETECT_SHORT] int  NOT NULL,
    [SUB_VER] char(4)  NOT NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL
);
GO

-- Creating table 'CONTROL_DATA'
CREATE TABLE [dbo].[CONTROL_DATA] (
    [T1] int  NOT NULL,
    [T2] int  NOT NULL,
    [T3] int  NOT NULL,
    [T4] int  NOT NULL,
    [T5] int  NOT NULL,
    [T6] int  NOT NULL,
    [T7] int  NOT NULL,
    [T8] int  NOT NULL,
    [BLOCK_REQ_TIME_OUT] int  NOT NULL,
    [SUB_VER] char(4)  NOT NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL
);
GO

-- Creating table 'SCALE_BASE_DATA'
CREATE TABLE [dbo].[SCALE_BASE_DATA] (
    [RESOLUTION] int  NOT NULL,
    [INPOSITION_AREA] int  NOT NULL,
    [INPOSITION_STABLE_TIME] int  NOT NULL,
    [TOTAL_SCALE_PULSE] int  NOT NULL,
    [SCALE_OFFSET] int  NOT NULL,
    [SCALE_RESE_DIST] int  NOT NULL,
    [READ_DIR] int  NOT NULL,
    [SUB_VER] char(4)  NOT NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL
);
GO

-- Creating table 'ASYSEXCUTEQUALITY'
CREATE TABLE [dbo].[ASYSEXCUTEQUALITY] (
    [CMD_ID_MCS] char(64)  NOT NULL,
    [CMD_INSERT_TIME] datetime  NOT NULL,
    [CMD_START_TIME] datetime  NULL,
    [CMD_FINISH_TIME] datetime  NULL,
    [CMD_FINISH_STATUS] int  NULL,
    [VH_ID] char(32)  NULL,
    [VH_START_SEC_ID] char(5)  NULL,
    [SOURCE_ADR] char(64)  NULL,
    [SEC_CNT_TO_SOURCE] int  NOT NULL,
    [SEC_DIS_TO_SOURCE] int  NOT NULL,
    [DESTINATION_ADR] char(64)  NULL,
    [SEC_CNT_TO_DESTN] int  NOT NULL,
    [SEC_DIS_TO_DESTN] int  NOT NULL,
    [CMDQUEUE_TIME] float  NOT NULL,
    [MOVE_TO_SOURCE_TIME] float  NOT NULL,
    [TOTAL_BLOCK_TIME_TO_SOURCE] float  NOT NULL,
    [TOTAL_OCS_TIME_TO_SOURCE] float  NOT NULL,
    [TOTAL_BLOCK_COUNT_TO_SOURCE] int  NOT NULL,
    [TOTAL_OCS_COUNT_TO_SOURCE] int  NOT NULL,
    [MOVE_TO_DESTN_TIME] float  NOT NULL,
    [TOTAL_BLOCK_TIME_TO_DESTN] float  NOT NULL,
    [TOTAL_OCS_TIME_TO_DESTN] float  NOT NULL,
    [TOTAL_BLOCK_COUNT_TO_DESTN] int  NOT NULL,
    [TOTAL_OCS_COUNT_TO_DESTN] int  NOT NULL,
    [TOTALPAUSE_TIME] float  NOT NULL,
    [CMD_TOTAL_EXCUTION_TIME] float  NOT NULL,
    [TOTAL_ACT_VH_COUNT] int  NOT NULL,
    [PARKING_VH_COUNT] int  NOT NULL,
    [CYCLERUN_VH_COUNT] int  NOT NULL,
    [TOTAL_IDLE_VH_COUNT] int  NOT NULL
);
GO

-- Creating table 'AVEHICLE'
CREATE TABLE [dbo].[AVEHICLE] (
    [VEHICLE_ID] char(32)  NOT NULL,
    [VEHICLE_TYPE] int  NOT NULL,
    [CUR_ADR_ID] char(5)  NULL,
    [CUR_SEC_ID] char(5)  NULL,
    [SEC_ENTRY_TIME] datetime  NULL,
    [ACC_SEC_DIST] float  NOT NULL,
    [MODE_STATUS] int  NOT NULL,
    [ACT_STATUS] int  NOT NULL,
    [MCS_CMD] char(64)  NULL,
    [OHTC_CMD] char(64)  NULL,
    [BLOCK_PAUSE] int  NOT NULL,
    [CMD_PAUSE] int  NOT NULL,
    [OBS_PAUSE] int  NOT NULL,
    [HID_PAUSE] int  NOT NULL,
    [ERROR] int  NOT NULL,
    [EARTHQUAKE_PAUSE] int  NOT NULL,
    [SAFETY_DOOR_PAUSE] int  NOT NULL,
    [OHXC_OBS_PAUSE] int  NOT NULL,
    [OHXC_BLOCK_PAUSE] int  NOT NULL,
    [OBS_DIST] int  NOT NULL,
    [HAS_CST] int  NOT NULL,
    [CST_ID] char(64)  NULL,
    [BOX_ID] char(64)  NULL,
    [LOT_ID] char(64)  NULL,
    [UPD_TIME] datetime  NULL,
    [VEHICLE_ACC_DIST] int  NOT NULL,
    [MANT_ACC_DIST] int  NOT NULL,
    [MANT_DATE] datetime  NULL,
    [GRIP_COUNT] int  NOT NULL,
    [GRIP_MANT_COUNT] int  NOT NULL,
    [GRIP_MANT_DATE] datetime  NULL,
    [NODE_ADR] char(5)  NULL,
    [IS_PARKING] bit  NOT NULL,
    [PARK_TIME] datetime  NULL,
    [PARK_ADR_ID] char(5)  NULL,
    [IS_CYCLING] bit  NOT NULL,
    [CYCLERUN_TIME] datetime  NULL,
    [CYCLERUN_ID] char(10)  NULL,
    [IS_INSTALLED] bit  NOT NULL,
    [INSTALLED_TIME] datetime  NULL,
    [REMOVED_TIME] datetime  NULL
);
GO

-- Creating table 'AHIDZONEDETAIL'
CREATE TABLE [dbo].[AHIDZONEDETAIL] (
    [ENTRY_SEC_ID] char(5)  NOT NULL,
    [SEC_ID] char(5)  NOT NULL
);
GO

-- Creating table 'AHIDZONEMASTER'
CREATE TABLE [dbo].[AHIDZONEMASTER] (
    [ENTRY_SEC_ID] char(5)  NOT NULL,
    [LEAVE_ADR_ID_1] char(5)  NOT NULL,
    [LEAVE_ADR_ID_2] char(5)  NULL,
    [MAX_LOAD_COUNT] int  NOT NULL
);
GO

-- Creating table 'AHIDZONEQUEUE'
CREATE TABLE [dbo].[AHIDZONEQUEUE] (
    [ENTRY_SEC_ID] char(5)  NOT NULL,
    [VEHICLE_ID] char(10)  NOT NULL,
    [REQ_TIME] datetime  NOT NULL,
    [BLOCK_TIME] datetime  NULL,
    [THROU_TIME] datetime  NULL,
    [RELEASE_TIME] datetime  NULL,
    [STATUS] int  NOT NULL,
    [IS_PASUE] bit  NOT NULL
);
GO

-- Creating table 'ALARMRPTCOND'
CREATE TABLE [dbo].[ALARMRPTCOND] (
    [EQPT_ID] char(15)  NOT NULL,
    [ALAM_CODE] char(10)  NOT NULL,
    [ENABLE_FLG] char(1)  NOT NULL
);
GO

-- Creating table 'ALARM'
CREATE TABLE [dbo].[ALARM] (
    [EQPT_ID] char(15)  NOT NULL,
    [UNIT_NUM] int  NOT NULL,
    [RPT_DATE_TIME] nvarchar(max)  NOT NULL,
    [ALAM_CODE] char(10)  NULL,
    [ALAM_LVL] int  NOT NULL,
    [ALAM_STAT] int  NOT NULL,
    [ALAM_DESC] char(80)  NULL,
    [ERROR_ID] char(64)  NULL,
    [UnitID] varchar(2)  NULL,
    [UnitState] varchar(2)  NULL
);
GO

-- Creating table 'ALARMMAP'
CREATE TABLE [dbo].[ALARMMAP] (
    [EQPT_ID] char(5)  NOT NULL,
    [ALARM_ID] char(10)  NOT NULL,
    [ALARM_LVL] int  NOT NULL,
    [ALARM_DESC] nvarchar(max)  NULL
);
GO

-- Creating table 'ZoneDef'
CREATE TABLE [dbo].[ZoneDef] (
    [StockerID] varchar(64)  NOT NULL,
    [ZoneID] varchar(64)  NOT NULL,
    [ZoneName] varchar(100)  NULL,
    [HighWaterMark] decimal(5,0)  NULL,
    [LowWaterMark] decimal(5,0)  NULL,
    [ZoneType] decimal(2,0)  NULL,
    [InventoryCheck] varchar(1)  NULL,
    [Color] varchar(15)  NULL,
    [SourceWeighted] decimal(2,0)  NULL,
    [DestWeighted] decimal(2,0)  NULL,
    [Remark] varchar(100)  NULL,
    [TrnDT] varchar(20)  NULL
);
GO

-- Creating table 'ACMD_MCS'
CREATE TABLE [dbo].[ACMD_MCS] (
    [CMD_ID] varchar(64)  NOT NULL,
    [CARRIER_ID] varchar(64)  NULL,
    [TRANSFERSTATE] int  NOT NULL,
    [COMMANDSTATE] int  NOT NULL,
    [HOSTSOURCE] varchar(64)  NULL,
    [HOSTDESTINATION] varchar(64)  NOT NULL,
    [PRIORITY] int  NOT NULL,
    [CHECKCODE] char(2)  NOT NULL,
    [PAUSEFLAG] char(1)  NOT NULL,
    [CMD_INSER_TIME] datetime  NOT NULL,
    [CMD_START_TIME] datetime  NULL,
    [CMD_FINISH_TIME] datetime  NULL,
    [TIME_PRIORITY] int  NOT NULL,
    [PORT_PRIORITY] int  NOT NULL,
    [REPLACE] int  NOT NULL,
    [PRIORITY_SUM] int  NOT NULL,
    [BOX_ID] varchar(64)  NULL,
    [CARRIER_LOC] varchar(64)  NULL,
    [LOT_ID] varchar(64)  NOT NULL,
    [CARRIER_ID_ON_CRANE] varchar(64)  NULL,
    [CMDTYPE] varchar(64)  NULL,
    [CRANE] varchar(64)  NOT NULL
);
GO

-- Creating table 'CassetteData'
CREATE TABLE [dbo].[CassetteData] (
    [StockerID] varchar(64)  NULL,
    [CSTID] varchar(64)  NULL,
    [BOXID] varchar(64)  NOT NULL,
    [Carrier_LOC] varchar(64)  NOT NULL,
    [Stage] decimal(2,0)  NOT NULL,
    [CSTState] int  NOT NULL,
    [LotID] varchar(64)  NULL,
    [EmptyCST] varchar(2)  NULL,
    [CSTType] varchar(2)  NULL,
    [CSTInDT] varchar(25)  NULL,
    [StoreDT] varchar(25)  NULL,
    [WaitOutOPDT] varchar(25)  NULL,
    [WaitOutLPDT] varchar(25)  NULL,
    [TrnDT] varchar(25)  NULL,
    [ReadStatus] varchar(64)  NULL
);
GO

-- Creating table 'ShelfDef'
CREATE TABLE [dbo].[ShelfDef] (
    [StockerID] varchar(64)  NOT NULL,
    [ShelfID] varchar(7)  NOT NULL,
    [Stage] decimal(2,0)  NOT NULL,
    [Bank_X] varchar(2)  NULL,
    [Bay_Y] varchar(3)  NULL,
    [Level_Z] varchar(2)  NULL,
    [LocateCraneNo] decimal(2,0)  NULL,
    [ShelfType] decimal(2,0)  NULL,
    [ShelfState] varchar(1)  NULL,
    [ZoneID] varchar(64)  NULL,
    [Enable] varchar(1)  NULL,
    [OldEnableSts] varchar(1)  NULL,
    [EmptyBlockFlag] varchar(1)  NULL,
    [HoldState] decimal(1,0)  NULL,
    [BCRReadFlag] varchar(1)  NULL,
    [TransferTimes] decimal(10,0)  NULL,
    [ChargeLoc] varchar(1)  NULL,
    [SelectPriority] decimal(2,0)  NULL,
    [Remark] varchar(100)  NULL,
    [TrnDT] varchar(25)  NULL,
    [HandoffDirection] decimal(1,0)  NULL,
    [ADR_ID] char(5)  NULL
);
GO

-- Creating table 'VACMD_MCS'
CREATE TABLE [dbo].[VACMD_MCS] (
    [CMD_ID] varchar(64)  NOT NULL,
    [CARRIER_ID] varchar(64)  NOT NULL,
    [TRANSFERSTATE] int  NOT NULL,
    [COMMANDSTATE] int  NOT NULL,
    [HOSTSOURCE] varchar(64)  NULL,
    [HOSTDESTINATION] varchar(64)  NOT NULL,
    [PRIORITY_SUM] int  NOT NULL,
    [PRIORITY] int  NOT NULL,
    [CHECKCODE] char(2)  NOT NULL,
    [PAUSEFLAG] char(1)  NOT NULL,
    [CMD_INSER_TIME] datetime  NOT NULL,
    [CMD_START_TIME] datetime  NULL,
    [CMD_FINISH_TIME] datetime  NULL,
    [TIME_PRIORITY] int  NOT NULL,
    [PORT_PRIORITY] int  NOT NULL,
    [REPLACE] int  NOT NULL,
    [OHTC_CMD] char(64)  NULL,
    [VH_ID] char(32)  NULL
);
GO

-- Creating table 'VSECTION_100'
CREATE TABLE [dbo].[VSECTION_100] (
    [SEC_ID] char(5)  NOT NULL,
    [SEC_ORDER_NUM] int  NULL,
    [SEG_ORDER_NUM] int  NOT NULL,
    [DIRC_DRIV] int  NOT NULL,
    [DIRC_GUID] int  NOT NULL,
    [SEG_NUM] char(3)  NULL,
    [FROM_ADR_ID] char(5)  NULL,
    [TO_ADR_ID] char(5)  NULL,
    [SEC_DIS] float  NOT NULL,
    [SEC_SPD] float  NULL,
    [DIS_FROM_ORIGIN] int  NOT NULL,
    [CDOG_1] int  NOT NULL,
    [CHG_SEG_NUM_1] char(3)  NULL,
    [CDOG_2] int  NOT NULL,
    [CHG_SEG_NUM_2] char(3)  NULL,
    [PRE_BLO_REQ] int  NOT NULL,
    [SEC_TYPE] int  NOT NULL,
    [SEC_DIR] int  NOT NULL,
    [PADDING] int  NOT NULL,
    [ENB_CHG_G_AREA] int  NOT NULL,
    [PRE_DIV] int  NOT NULL,
    [PRE_ADD_REPR] int  NOT NULL,
    [OBS_SENSOR] int  NOT NULL,
    [SUB_VER] char(4)  NOT NULL,
    [ADD_TIME] datetime  NULL,
    [ADD_USER] char(10)  NULL,
    [UPD_TIME] datetime  NULL,
    [UPD_USER] char(10)  NULL,
    [START_BC1] int  NOT NULL,
    [END_BC1] int  NOT NULL,
    [START_BC2] int  NOT NULL,
    [END_BC2] int  NOT NULL,
    [START_BC3] int  NOT NULL,
    [END_BC3] int  NOT NULL,
    [CHG_AREA_SECSOR_1] int  NOT NULL,
    [CHG_AREA_SECSOR_2] int  NOT NULL,
    [OBS_SENSOR_F] int  NOT NULL,
    [OBS_SENSOR_R] int  NOT NULL,
    [OBS_SENSOR_L] int  NOT NULL,
    [RANGE_SENSOR_F] int  NOT NULL,
    [IS_ADR_RPT] bit  NOT NULL,
    [CAN_GUIDE_CHG] bit  NOT NULL,
    [HID_CONTROL] bit  NOT NULL,
    [BRANCH_FLAG] bit  NOT NULL,
    [AREA_SECSOR] int  NULL,
    [LAST_TECH_TIME] datetime  NULL
);
GO

-- Creating table 'PortDef'
CREATE TABLE [dbo].[PortDef] (
    [StockerID] varchar(64)  NULL,
    [PLCPortID] varchar(64)  NOT NULL,
    [OHBName] varchar(20)  NULL,
    [State] int  NOT NULL,
    [Stage] decimal(2,0)  NULL,
    [AGVState] int  NULL,
    [UnitType] varchar(20)  NULL,
    [HostEQPortID] varchar(64)  NULL,
    [ShelfID] varchar(7)  NULL,
    [PortType] int  NOT NULL,
    [PortLocationType] decimal(2,0)  NULL,
    [PortTypeIndex] decimal(2,0)  NULL,
    [SourceWeighted] decimal(2,0)  NULL,
    [DestWeighted] decimal(2,0)  NULL,
    [Direction] decimal(1,0)  NULL,
    [Color] decimal(6,0)  NULL,
    [TimeOutForAutoUD] decimal(6,0)  NULL,
    [TimeOutForAutoInZone] varchar(64)  NULL,
    [AlternateToZone] varchar(64)  NULL,
    [Vehicles] decimal(1,0)  NULL,
    [Floor] decimal(1,0)  NULL,
    [IgnoreModeChange] varchar(1)  NULL,
    [ReportMCSFlag] varchar(1)  NULL,
    [ReportStage] decimal(2,0)  NULL,
    [NetHStnNo] decimal(2,0)  NULL,
    [AreaSensorStnNo] decimal(2,0)  NULL,
    [Presenton_InsCST] varchar(1)  NULL,
    [Presentoff_DelCST] varchar(1)  NULL,
    [ToEQ] varchar(64)  NULL,
    [TrnDT] varchar(20)  NULL,
    [AlarmType] decimal(2,0)  NULL,
    [ADR_ID] char(5)  NOT NULL,
    [PortGroup] int  NULL,
    [PortTypeDef] int  NULL,
    [PRIORITY] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [VER_TYPE], [SUB_VER] in table 'ABASEDATA_VER'
ALTER TABLE [dbo].[ABASEDATA_VER]
ADD CONSTRAINT [PK_ABASEDATA_VER]
    PRIMARY KEY CLUSTERED ([VER_TYPE], [SUB_VER] ASC);
GO

-- Creating primary key on [SEC_ID], [ENTRY_SEC_ID] in table 'ABLOCKZONEDETAIL'
ALTER TABLE [dbo].[ABLOCKZONEDETAIL]
ADD CONSTRAINT [PK_ABLOCKZONEDETAIL]
    PRIMARY KEY CLUSTERED ([SEC_ID], [ENTRY_SEC_ID] ASC);
GO

-- Creating primary key on [SECTION_ID], [RAIL_ID] in table 'AGROUPRAILS'
ALTER TABLE [dbo].[AGROUPRAILS]
ADD CONSTRAINT [PK_AGROUPRAILS]
    PRIMARY KEY CLUSTERED ([SECTION_ID], [RAIL_ID] ASC);
GO

-- Creating primary key on [MAIN_VER] in table 'AMAIN_VER'
ALTER TABLE [dbo].[AMAIN_VER]
ADD CONSTRAINT [PK_AMAIN_VER]
    PRIMARY KEY CLUSTERED ([MAIN_VER] ASC);
GO

-- Creating primary key on [POINT_ID] in table 'APOINT'
ALTER TABLE [dbo].[APOINT]
ADD CONSTRAINT [PK_APOINT]
    PRIMARY KEY CLUSTERED ([POINT_ID] ASC);
GO

-- Creating primary key on [RAIL_ID] in table 'ARAIL'
ALTER TABLE [dbo].[ARAIL]
ADD CONSTRAINT [PK_ARAIL]
    PRIMARY KEY CLUSTERED ([RAIL_ID] ASC);
GO

-- Creating primary key on [SEG_NUM] in table 'ASEGMENT'
ALTER TABLE [dbo].[ASEGMENT]
ADD CONSTRAINT [PK_ASEGMENT]
    PRIMARY KEY CLUSTERED ([SEG_NUM] ASC);
GO

-- Creating primary key on [MAIN_VER], [VER_TYPE] in table 'ASUB_VER'
ALTER TABLE [dbo].[ASUB_VER]
ADD CONSTRAINT [PK_ASUB_VER]
    PRIMARY KEY CLUSTERED ([MAIN_VER], [VER_TYPE] ASC);
GO

-- Creating primary key on [CAR_ID], [ENTRY_SEC_ID], [REQ_TIME] in table 'BLOCKZONEQUEUE'
ALTER TABLE [dbo].[BLOCKZONEQUEUE]
ADD CONSTRAINT [PK_BLOCKZONEQUEUE]
    PRIMARY KEY CLUSTERED ([CAR_ID], [ENTRY_SEC_ID], [REQ_TIME] ASC);
GO

-- Creating primary key on [ENTRY_SEC_ID] in table 'ABLOCKZONEMASTER'
ALTER TABLE [dbo].[ABLOCKZONEMASTER]
ADD CONSTRAINT [PK_ABLOCKZONEMASTER]
    PRIMARY KEY CLUSTERED ([ENTRY_SEC_ID] ASC);
GO

-- Creating primary key on [SEQ_NAME] in table 'ASEQUENCE'
ALTER TABLE [dbo].[ASEQUENCE]
ADD CONSTRAINT [PK_ASEQUENCE]
    PRIMARY KEY CLUSTERED ([SEQ_NAME] ASC);
GO

-- Creating primary key on [CYCLE_ZONE_ID], [SEC_ID] in table 'ACYCLEZONEDETAIL'
ALTER TABLE [dbo].[ACYCLEZONEDETAIL]
ADD CONSTRAINT [PK_ACYCLEZONEDETAIL]
    PRIMARY KEY CLUSTERED ([CYCLE_ZONE_ID], [SEC_ID] ASC);
GO

-- Creating primary key on [CYCLE_TYPE_ID], [CYCLE_ZONE_ID] in table 'ACYCLEZONEMASTER'
ALTER TABLE [dbo].[ACYCLEZONEMASTER]
ADD CONSTRAINT [PK_ACYCLEZONEMASTER]
    PRIMARY KEY CLUSTERED ([CYCLE_TYPE_ID], [CYCLE_ZONE_ID] ASC);
GO

-- Creating primary key on [CYCLE_TYPE_ID] in table 'ACYCLEZONETYPE'
ALTER TABLE [dbo].[ACYCLEZONETYPE]
ADD CONSTRAINT [PK_ACYCLEZONETYPE]
    PRIMARY KEY CLUSTERED ([CYCLE_TYPE_ID] ASC);
GO

-- Creating primary key on [ADR_ID] in table 'AADDRESS'
ALTER TABLE [dbo].[AADDRESS]
ADD CONSTRAINT [PK_AADDRESS]
    PRIMARY KEY CLUSTERED ([ADR_ID] ASC);
GO

-- Creating primary key on [CMD_ID] in table 'ACMD_OHTC'
ALTER TABLE [dbo].[ACMD_OHTC]
ADD CONSTRAINT [PK_ACMD_OHTC]
    PRIMARY KEY CLUSTERED ([CMD_ID] ASC);
GO

-- Creating primary key on [CEID], [RPTID] in table 'ACEID'
ALTER TABLE [dbo].[ACEID]
ADD CONSTRAINT [PK_ACEID]
    PRIMARY KEY CLUSTERED ([CEID], [RPTID] ASC);
GO

-- Creating primary key on [RPTID], [VID] in table 'ARPTID'
ALTER TABLE [dbo].[ARPTID]
ADD CONSTRAINT [PK_ARPTID]
    PRIMARY KEY CLUSTERED ([RPTID], [VID] ASC);
GO

-- Creating primary key on [EQ_ID] in table 'AVIDINFO'
ALTER TABLE [dbo].[AVIDINFO]
ADD CONSTRAINT [PK_AVIDINFO]
    PRIMARY KEY CLUSTERED ([EQ_ID] ASC);
GO

-- Creating primary key on [VEICLE_ID], [ADR_ID], [SEC_ID], [UPD_TIME] in table 'ANETWORKQUALITY'
ALTER TABLE [dbo].[ANETWORKQUALITY]
ADD CONSTRAINT [PK_ANETWORKQUALITY]
    PRIMARY KEY CLUSTERED ([VEICLE_ID], [ADR_ID], [SEC_ID], [UPD_TIME] ASC);
GO

-- Creating primary key on [PORT_ID] in table 'APORTICON'
ALTER TABLE [dbo].[APORTICON]
ADD CONSTRAINT [PK_APORTICON]
    PRIMARY KEY CLUSTERED ([PORT_ID] ASC);
GO

-- Creating primary key on [BC_ID] in table 'BCSTAT'
ALTER TABLE [dbo].[BCSTAT]
ADD CONSTRAINT [PK_BCSTAT]
    PRIMARY KEY CLUSTERED ([BC_ID] ASC);
GO

-- Creating primary key on [EQPT_ID] in table 'AEQPT'
ALTER TABLE [dbo].[AEQPT]
ADD CONSTRAINT [PK_AEQPT]
    PRIMARY KEY CLUSTERED ([EQPT_ID] ASC);
GO

-- Creating primary key on [LINE_ID] in table 'ALINE'
ALTER TABLE [dbo].[ALINE]
ADD CONSTRAINT [PK_ALINE]
    PRIMARY KEY CLUSTERED ([LINE_ID] ASC);
GO

-- Creating primary key on [ZONE_ID] in table 'AZONE'
ALTER TABLE [dbo].[AZONE]
ADD CONSTRAINT [PK_AZONE]
    PRIMARY KEY CLUSTERED ([ZONE_ID] ASC);
GO

-- Creating primary key on [NODE_ID] in table 'ANODE'
ALTER TABLE [dbo].[ANODE]
ADD CONSTRAINT [PK_ANODE]
    PRIMARY KEY CLUSTERED ([NODE_ID] ASC);
GO

-- Creating primary key on [UNIT_ID] in table 'AUNIT'
ALTER TABLE [dbo].[AUNIT]
ADD CONSTRAINT [PK_AUNIT]
    PRIMARY KEY CLUSTERED ([UNIT_ID] ASC);
GO

-- Creating primary key on [BUFF_ID] in table 'ABUFFER'
ALTER TABLE [dbo].[ABUFFER]
ADD CONSTRAINT [PK_ABUFFER]
    PRIMARY KEY CLUSTERED ([BUFF_ID] ASC);
GO

-- Creating primary key on [CST_ID] in table 'ACASSETTE'
ALTER TABLE [dbo].[ACASSETTE]
ADD CONSTRAINT [PK_ACASSETTE]
    PRIMARY KEY CLUSTERED ([CST_ID] ASC);
GO

-- Creating primary key on [CRATE_ID] in table 'ACRATE'
ALTER TABLE [dbo].[ACRATE]
ADD CONSTRAINT [PK_ACRATE]
    PRIMARY KEY CLUSTERED ([CRATE_ID] ASC);
GO

-- Creating primary key on [ECID] in table 'AECDATAMAP'
ALTER TABLE [dbo].[AECDATAMAP]
ADD CONSTRAINT [PK_AECDATAMAP]
    PRIMARY KEY CLUSTERED ([ECID] ASC);
GO

-- Creating primary key on [CEID] in table 'AEVENTRPTCOND'
ALTER TABLE [dbo].[AEVENTRPTCOND]
ADD CONSTRAINT [PK_AEVENTRPTCOND]
    PRIMARY KEY CLUSTERED ([CEID] ASC);
GO

-- Creating primary key on [UPSTREAM_ID], [DOWNSTREAM_ID] in table 'AFLOW_REL'
ALTER TABLE [dbo].[AFLOW_REL]
ADD CONSTRAINT [PK_AFLOW_REL]
    PRIMARY KEY CLUSTERED ([UPSTREAM_ID], [DOWNSTREAM_ID] ASC);
GO

-- Creating primary key on [FUNC_CODE] in table 'UASFNC'
ALTER TABLE [dbo].[UASFNC]
ADD CONSTRAINT [PK_UASFNC]
    PRIMARY KEY CLUSTERED ([FUNC_CODE] ASC);
GO

-- Creating primary key on [LOT_ID] in table 'ALOT'
ALTER TABLE [dbo].[ALOT]
ADD CONSTRAINT [PK_ALOT]
    PRIMARY KEY CLUSTERED ([LOT_ID] ASC);
GO

-- Creating primary key on [SEQ_NO] in table 'HOPERATION'
ALTER TABLE [dbo].[HOPERATION]
ADD CONSTRAINT [PK_HOPERATION]
    PRIMARY KEY CLUSTERED ([SEQ_NO] ASC);
GO

-- Creating primary key on [SHT_ID] in table 'ASHEET'
ALTER TABLE [dbo].[ASHEET]
ADD CONSTRAINT [PK_ASHEET]
    PRIMARY KEY CLUSTERED ([SHT_ID] ASC);
GO

-- Creating primary key on [SEQ_NO] in table 'HASHEET'
ALTER TABLE [dbo].[HASHEET]
ADD CONSTRAINT [PK_HASHEET]
    PRIMARY KEY CLUSTERED ([SEQ_NO] ASC);
GO

-- Creating primary key on [TRACE_ID], [SVID] in table 'ATRACEITEM'
ALTER TABLE [dbo].[ATRACEITEM]
ADD CONSTRAINT [PK_ATRACEITEM]
    PRIMARY KEY CLUSTERED ([TRACE_ID], [SVID] ASC);
GO

-- Creating primary key on [TRACE_ID] in table 'ATRACESET'
ALTER TABLE [dbo].[ATRACESET]
ADD CONSTRAINT [PK_ATRACESET]
    PRIMARY KEY CLUSTERED ([TRACE_ID] ASC);
GO

-- Creating primary key on [USER_ID] in table 'UASUSR'
ALTER TABLE [dbo].[UASUSR]
ADD CONSTRAINT [PK_UASUSR]
    PRIMARY KEY CLUSTERED ([USER_ID] ASC);
GO

-- Creating primary key on [USER_GRP] in table 'UASUSRGRP'
ALTER TABLE [dbo].[UASUSRGRP]
ADD CONSTRAINT [PK_UASUSRGRP]
    PRIMARY KEY CLUSTERED ([USER_GRP] ASC);
GO

-- Creating primary key on [USER_GRP], [FUNC_CODE] in table 'UASUFNC'
ALTER TABLE [dbo].[UASUFNC]
ADD CONSTRAINT [PK_UASUFNC]
    PRIMARY KEY CLUSTERED ([USER_GRP], [FUNC_CODE] ASC);
GO

-- Creating primary key on [PORT_ID] in table 'APORTSTATION'
ALTER TABLE [dbo].[APORTSTATION]
ADD CONSTRAINT [PK_APORTSTATION]
    PRIMARY KEY CLUSTERED ([PORT_ID] ASC);
GO

-- Creating primary key on [PORT_ID] in table 'APORT'
ALTER TABLE [dbo].[APORT]
ADD CONSTRAINT [PK_APORT]
    PRIMARY KEY CLUSTERED ([PORT_ID] ASC);
GO

-- Creating primary key on [ID] in table 'AMCSREPORTQUEUE'
ALTER TABLE [dbo].[AMCSREPORTQUEUE]
ADD CONSTRAINT [PK_AMCSREPORTQUEUE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ADR_ID], [VEHOCLE_ID] in table 'AADDRESS_DATA'
ALTER TABLE [dbo].[AADDRESS_DATA]
ADD CONSTRAINT [PK_AADDRESS_DATA]
    PRIMARY KEY CLUSTERED ([ADR_ID], [VEHOCLE_ID] ASC);
GO

-- Creating primary key on [CMD_ID], [SEQ_NO] in table 'ACMD_OHTC_DETAIL'
ALTER TABLE [dbo].[ACMD_OHTC_DETAIL]
ADD CONSTRAINT [PK_ACMD_OHTC_DETAIL]
    PRIMARY KEY CLUSTERED ([CMD_ID], [SEQ_NO] ASC);
GO

-- Creating primary key on [PARK_ZONE_ID], [ADR_ID] in table 'APARKZONEDETAIL'
ALTER TABLE [dbo].[APARKZONEDETAIL]
ADD CONSTRAINT [PK_APARKZONEDETAIL]
    PRIMARY KEY CLUSTERED ([PARK_ZONE_ID], [ADR_ID] ASC);
GO

-- Creating primary key on [PARK_TYPE_ID], [PARK_ZONE_ID] in table 'APARKZONEMASTER'
ALTER TABLE [dbo].[APARKZONEMASTER]
ADD CONSTRAINT [PK_APARKZONEMASTER]
    PRIMARY KEY CLUSTERED ([PARK_TYPE_ID], [PARK_ZONE_ID] ASC);
GO

-- Creating primary key on [PARK_TYPE_ID] in table 'APARKZONETYPE'
ALTER TABLE [dbo].[APARKZONETYPE]
ADD CONSTRAINT [PK_APARKZONETYPE]
    PRIMARY KEY CLUSTERED ([PARK_TYPE_ID] ASC);
GO

-- Creating primary key on [PORT_ID], [VH_ID], [SUB_VER] in table 'APORT_POSITION_TEACHING_DATA'
ALTER TABLE [dbo].[APORT_POSITION_TEACHING_DATA]
ADD CONSTRAINT [PK_APORT_POSITION_TEACHING_DATA]
    PRIMARY KEY CLUSTERED ([PORT_ID], [VH_ID], [SUB_VER] ASC);
GO

-- Creating primary key on [SEC_ID], [SUB_VER] in table 'ASECTION'
ALTER TABLE [dbo].[ASECTION]
ADD CONSTRAINT [PK_ASECTION]
    PRIMARY KEY CLUSTERED ([SEC_ID], [SUB_VER] ASC);
GO

-- Creating primary key on [SEC_ID], [SUB_VER] in table 'ASECTION_CONTROL_100'
ALTER TABLE [dbo].[ASECTION_CONTROL_100]
ADD CONSTRAINT [PK_ASECTION_CONTROL_100]
    PRIMARY KEY CLUSTERED ([SEC_ID], [SUB_VER] ASC);
GO

-- Creating primary key on [VEHICLE_ID], [SUB_VER] in table 'AVEHICLE_CONTROL_100'
ALTER TABLE [dbo].[AVEHICLE_CONTROL_100]
ADD CONSTRAINT [PK_AVEHICLE_CONTROL_100]
    PRIMARY KEY CLUSTERED ([VEHICLE_ID], [SUB_VER] ASC);
GO

-- Creating primary key on [SUB_VER] in table 'CONTROL_DATA'
ALTER TABLE [dbo].[CONTROL_DATA]
ADD CONSTRAINT [PK_CONTROL_DATA]
    PRIMARY KEY CLUSTERED ([SUB_VER] ASC);
GO

-- Creating primary key on [SUB_VER] in table 'SCALE_BASE_DATA'
ALTER TABLE [dbo].[SCALE_BASE_DATA]
ADD CONSTRAINT [PK_SCALE_BASE_DATA]
    PRIMARY KEY CLUSTERED ([SUB_VER] ASC);
GO

-- Creating primary key on [CMD_ID_MCS] in table 'ASYSEXCUTEQUALITY'
ALTER TABLE [dbo].[ASYSEXCUTEQUALITY]
ADD CONSTRAINT [PK_ASYSEXCUTEQUALITY]
    PRIMARY KEY CLUSTERED ([CMD_ID_MCS] ASC);
GO

-- Creating primary key on [VEHICLE_ID] in table 'AVEHICLE'
ALTER TABLE [dbo].[AVEHICLE]
ADD CONSTRAINT [PK_AVEHICLE]
    PRIMARY KEY CLUSTERED ([VEHICLE_ID] ASC);
GO

-- Creating primary key on [ENTRY_SEC_ID], [SEC_ID] in table 'AHIDZONEDETAIL'
ALTER TABLE [dbo].[AHIDZONEDETAIL]
ADD CONSTRAINT [PK_AHIDZONEDETAIL]
    PRIMARY KEY CLUSTERED ([ENTRY_SEC_ID], [SEC_ID] ASC);
GO

-- Creating primary key on [ENTRY_SEC_ID] in table 'AHIDZONEMASTER'
ALTER TABLE [dbo].[AHIDZONEMASTER]
ADD CONSTRAINT [PK_AHIDZONEMASTER]
    PRIMARY KEY CLUSTERED ([ENTRY_SEC_ID] ASC);
GO

-- Creating primary key on [ENTRY_SEC_ID], [VEHICLE_ID], [REQ_TIME] in table 'AHIDZONEQUEUE'
ALTER TABLE [dbo].[AHIDZONEQUEUE]
ADD CONSTRAINT [PK_AHIDZONEQUEUE]
    PRIMARY KEY CLUSTERED ([ENTRY_SEC_ID], [VEHICLE_ID], [REQ_TIME] ASC);
GO

-- Creating primary key on [EQPT_ID], [ALAM_CODE] in table 'ALARMRPTCOND'
ALTER TABLE [dbo].[ALARMRPTCOND]
ADD CONSTRAINT [PK_ALARMRPTCOND]
    PRIMARY KEY CLUSTERED ([EQPT_ID], [ALAM_CODE] ASC);
GO

-- Creating primary key on [EQPT_ID], [UNIT_NUM], [RPT_DATE_TIME] in table 'ALARM'
ALTER TABLE [dbo].[ALARM]
ADD CONSTRAINT [PK_ALARM]
    PRIMARY KEY CLUSTERED ([EQPT_ID], [UNIT_NUM], [RPT_DATE_TIME] ASC);
GO

-- Creating primary key on [EQPT_ID], [ALARM_ID], [ALARM_LVL] in table 'ALARMMAP'
ALTER TABLE [dbo].[ALARMMAP]
ADD CONSTRAINT [PK_ALARMMAP]
    PRIMARY KEY CLUSTERED ([EQPT_ID], [ALARM_ID], [ALARM_LVL] ASC);
GO

-- Creating primary key on [ZoneID] in table 'ZoneDef'
ALTER TABLE [dbo].[ZoneDef]
ADD CONSTRAINT [PK_ZoneDef]
    PRIMARY KEY CLUSTERED ([ZoneID] ASC);
GO

-- Creating primary key on [CMD_ID] in table 'ACMD_MCS'
ALTER TABLE [dbo].[ACMD_MCS]
ADD CONSTRAINT [PK_ACMD_MCS]
    PRIMARY KEY CLUSTERED ([CMD_ID] ASC);
GO

-- Creating primary key on [BOXID] in table 'CassetteData'
ALTER TABLE [dbo].[CassetteData]
ADD CONSTRAINT [PK_CassetteData]
    PRIMARY KEY CLUSTERED ([BOXID] ASC);
GO

-- Creating primary key on [ShelfID] in table 'ShelfDef'
ALTER TABLE [dbo].[ShelfDef]
ADD CONSTRAINT [PK_ShelfDef]
    PRIMARY KEY CLUSTERED ([ShelfID] ASC);
GO

-- Creating primary key on [CMD_ID], [CARRIER_ID], [TRANSFERSTATE], [COMMANDSTATE], [HOSTDESTINATION], [PRIORITY_SUM], [PRIORITY], [CHECKCODE], [PAUSEFLAG], [CMD_INSER_TIME], [TIME_PRIORITY], [PORT_PRIORITY], [REPLACE] in table 'VACMD_MCS'
ALTER TABLE [dbo].[VACMD_MCS]
ADD CONSTRAINT [PK_VACMD_MCS]
    PRIMARY KEY CLUSTERED ([CMD_ID], [CARRIER_ID], [TRANSFERSTATE], [COMMANDSTATE], [HOSTDESTINATION], [PRIORITY_SUM], [PRIORITY], [CHECKCODE], [PAUSEFLAG], [CMD_INSER_TIME], [TIME_PRIORITY], [PORT_PRIORITY], [REPLACE] ASC);
GO

-- Creating primary key on [SEC_ID], [SEG_ORDER_NUM], [DIRC_DRIV], [DIRC_GUID], [SEC_DIS], [DIS_FROM_ORIGIN], [CDOG_1], [CDOG_2], [PRE_BLO_REQ], [SEC_TYPE], [SEC_DIR], [PADDING], [ENB_CHG_G_AREA], [PRE_DIV], [PRE_ADD_REPR], [OBS_SENSOR], [SUB_VER], [START_BC1], [END_BC1], [START_BC2], [END_BC2], [START_BC3], [END_BC3], [CHG_AREA_SECSOR_1], [CHG_AREA_SECSOR_2], [OBS_SENSOR_F], [OBS_SENSOR_R], [OBS_SENSOR_L], [RANGE_SENSOR_F], [IS_ADR_RPT], [CAN_GUIDE_CHG], [HID_CONTROL], [BRANCH_FLAG] in table 'VSECTION_100'
ALTER TABLE [dbo].[VSECTION_100]
ADD CONSTRAINT [PK_VSECTION_100]
    PRIMARY KEY CLUSTERED ([SEC_ID], [SEG_ORDER_NUM], [DIRC_DRIV], [DIRC_GUID], [SEC_DIS], [DIS_FROM_ORIGIN], [CDOG_1], [CDOG_2], [PRE_BLO_REQ], [SEC_TYPE], [SEC_DIR], [PADDING], [ENB_CHG_G_AREA], [PRE_DIV], [PRE_ADD_REPR], [OBS_SENSOR], [SUB_VER], [START_BC1], [END_BC1], [START_BC2], [END_BC2], [START_BC3], [END_BC3], [CHG_AREA_SECSOR_1], [CHG_AREA_SECSOR_2], [OBS_SENSOR_F], [OBS_SENSOR_R], [OBS_SENSOR_L], [RANGE_SENSOR_F], [IS_ADR_RPT], [CAN_GUIDE_CHG], [HID_CONTROL], [BRANCH_FLAG] ASC);
GO

-- Creating primary key on [PLCPortID] in table 'PortDef'
ALTER TABLE [dbo].[PortDef]
ADD CONSTRAINT [PK_PortDef]
    PRIMARY KEY CLUSTERED ([PLCPortID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------