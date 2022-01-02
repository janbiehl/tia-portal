using PlcSimAdvanced.Protos;
using Siemens.Simatic.Simulation.Runtime;

namespace PlcSimAdvancedGateway;

public static class Utils
{
    public static MemoryArea GetMemoryArea(EArea plcMemoryArea)
    {
        return plcMemoryArea switch
        {
            EArea.Input => MemoryArea.MaInput,
            EArea.Marker => MemoryArea.MaMarker,
            EArea.Output => MemoryArea.MaOutput,
            EArea.Counter => MemoryArea.MaCounter,
            EArea.Timer => MemoryArea.MaTimer,
            EArea.DataBlock => MemoryArea.MaDataBlock,
            _ => MemoryArea.MaInvalid
        };
    }

    public static EArea GetMemoryArea(MemoryArea memoryArea)
    {
        return memoryArea switch
        {
            MemoryArea.MaInput => EArea.Input,
            MemoryArea.MaMarker => EArea.Marker,
            MemoryArea.MaOutput => EArea.Output,
            MemoryArea.MaCounter => EArea.Counter,
            MemoryArea.MaTimer => EArea.Timer,
            MemoryArea.MaDataBlock => EArea.DataBlock,
            _ => EArea.InvalidArea
        };
    }

    public static PlcDataType GetDatatype(EDataType dataType)
    {
        return dataType switch
        {
            EDataType.Bool => PlcDataType.DtBool,
            EDataType.Byte => PlcDataType.DtByte,
            EDataType.Char => PlcDataType.DtChar,
            EDataType.Word => PlcDataType.DtWord,
            EDataType.Int => PlcDataType.DtInt,
            EDataType.DWord => PlcDataType.DtDword,
            EDataType.DInt => PlcDataType.DtDint,
            EDataType.Real => PlcDataType.DtReal,
            EDataType.Date => PlcDataType.DtDate,
            EDataType.TimeOfDay => PlcDataType.DtTimeOfDay,
            EDataType.Time => PlcDataType.DtTime,
            EDataType.S5Time => PlcDataType.DtS5Time,
            EDataType.DateAndTime => PlcDataType.DtDateAndTime,
            EDataType.Struct => PlcDataType.DtStruct,
            EDataType.String => PlcDataType.DtString,
            EDataType.Counter => PlcDataType.DtCounter,
            EDataType.Timer => PlcDataType.DtTimer,
            EDataType.IEC_Counter => PlcDataType.DtIecCounter,
            EDataType.IEC_Timer => PlcDataType.DtIecTimer,
            EDataType.LReal => PlcDataType.DtLreal,
            EDataType.ULInt => PlcDataType.DtUlint,
            EDataType.LInt => PlcDataType.DtLint,
            EDataType.LWord => PlcDataType.DtLword,
            EDataType.USInt => PlcDataType.DtUsint,
            EDataType.UInt => PlcDataType.DtUint,
            EDataType.UDInt => PlcDataType.DtUdint,
            EDataType.SInt => PlcDataType.DtSint,
            EDataType.WChar => PlcDataType.DtWchar,
            EDataType.WString => PlcDataType.DtWstring,
            EDataType.LTime => PlcDataType.DtLtime,
            EDataType.LTimeOfDay => PlcDataType.DtLtimeOfDay,
            EDataType.LDT => PlcDataType.DtLdt,
            EDataType.DTL => PlcDataType.DtDtl,
            EDataType.IEC_LTimer => PlcDataType.DtIecLtimer,
            EDataType.IEC_SCounter => PlcDataType.DtIecScounter,
            EDataType.IEC_DCounter => PlcDataType.DtIecDcounter,
            EDataType.IEC_LCounter => PlcDataType.DtIecLcounter,
            EDataType.IEC_UCounter => PlcDataType.DtIecUcounter,
            EDataType.IEC_UDCounter => PlcDataType.DtIecUdcounter,
            EDataType.IEC_USCounter => PlcDataType.DtIecUscounter,
            EDataType.IEC_ULCounter => PlcDataType.DtIecUlcounter,
            EDataType.ErrorStruct => PlcDataType.DtErrorStruct,
            EDataType.NREF => PlcDataType.DtNref,
            EDataType.CREF => PlcDataType.DtCref,
            EDataType.Aom_Ident => PlcDataType.DtAomIdent,
            EDataType.Event_Any => PlcDataType.DtEventAny,
            EDataType.Event_Att => PlcDataType.DtEventAtt,
            EDataType.Event_HwInt => PlcDataType.DtEventHwInt,
            EDataType.Hw_Any => PlcDataType.DtHwAny,
            EDataType.Hw_IoSystem => PlcDataType.DtHwIoSystem,
            EDataType.Hw_DpMaster => PlcDataType.DtHwDpMaster,
            EDataType.Hw_Device => PlcDataType.DtHwDevice,
            EDataType.Hw_DpSlave => PlcDataType.DtHwDpSlave,
            EDataType.Hw_Io => PlcDataType.DtHwIo,
            EDataType.Hw_Module => PlcDataType.DtHwModule,
            EDataType.Hw_SubModule => PlcDataType.DtHwSubModule,
            EDataType.Hw_Hsc => PlcDataType.DtHwHsc,
            EDataType.Hw_Pwm => PlcDataType.DtHwPwm,
            EDataType.Hw_Pto => PlcDataType.DtHwPto,
            EDataType.Hw_Interface => PlcDataType.DtHwInterface,
            EDataType.Hw_IEPort => PlcDataType.DtHwIeport,
            EDataType.OB_Any => PlcDataType.DtObAny,
            EDataType.OB_Delay => PlcDataType.DtObDelay,
            EDataType.OB_Tod => PlcDataType.DtObTod,
            EDataType.OB_Cyclic => PlcDataType.DtObCyclic,
            EDataType.OB_Att => PlcDataType.DtObAtt,
            EDataType.Conn_Any => PlcDataType.DtConnAny,
            EDataType.Conn_Prg => PlcDataType.DtConnPrg,
            EDataType.Conn_Ouc => PlcDataType.DtConnOuc,
            EDataType.Conn_R_ID => PlcDataType.DtConnRId,
            EDataType.Port => PlcDataType.DtPort,
            EDataType.Rtm => PlcDataType.DtRtm,
            EDataType.Pip => PlcDataType.DtPip,
            EDataType.OB_PCycle => PlcDataType.DtObPcycle,
            EDataType.OB_HwInt => PlcDataType.DtObHwInt,
            EDataType.OB_Diag => PlcDataType.DtObDiag,
            EDataType.OB_TimeError => PlcDataType.DtObTimeError,
            EDataType.OB_Startup => PlcDataType.DtObStartup,
            EDataType.DB_Any => PlcDataType.DtDbAny,
            EDataType.DB_WWW => PlcDataType.DtDbWww,
            EDataType.DB_Dyn => PlcDataType.DtDbDyn,
            EDataType.DB => PlcDataType.DtDb,
            _ => PlcDataType.DtUnknown
        };
    }
    
    public static EDataType GetDatatype(PlcDataType plcDataType)
    {
        return plcDataType switch
        {
            PlcDataType.DtBool => EDataType.Bool,
            PlcDataType.DtByte => EDataType.Byte,
            PlcDataType.DtChar => EDataType.Char,
            PlcDataType.DtWord => EDataType.Word,
            PlcDataType.DtInt => EDataType.Int,
            PlcDataType.DtDword => EDataType.DWord,
            PlcDataType.DtDint => EDataType.DInt,
            PlcDataType.DtReal => EDataType.Real,
            PlcDataType.DtDate => EDataType.Date,          
            PlcDataType.DtTimeOfDay => EDataType.TimeOfDay,     
            PlcDataType.DtTime => EDataType.Time,          
            PlcDataType.DtS5Time => EDataType.S5Time,        
            PlcDataType.DtDateAndTime => EDataType.DateAndTime,   
            PlcDataType.DtStruct => EDataType.Struct,        
            PlcDataType.DtString => EDataType.String,        
            PlcDataType.DtCounter => EDataType.Counter,       
            PlcDataType.DtTimer => EDataType.Timer,         
            PlcDataType.DtIecCounter => EDataType.IEC_Counter,   
            PlcDataType.DtIecTimer => EDataType.IEC_Timer,     
            PlcDataType.DtLreal => EDataType.LReal,
            PlcDataType.DtUlint => EDataType.ULInt,
            PlcDataType.DtLint => EDataType.LInt ,
            PlcDataType.DtLword => EDataType.LWord,
            PlcDataType.DtUsint => EDataType.USInt,
            PlcDataType.DtUint => EDataType.UInt ,
            PlcDataType.DtUdint => EDataType.UDInt,
            PlcDataType.DtSint => EDataType.SInt ,
            PlcDataType.DtWchar => EDataType.WChar,
            PlcDataType.DtWstring => EDataType.WString,
            PlcDataType.DtLtime => EDataType.LTime,
            PlcDataType.DtLtimeOfDay => EDataType.LTimeOfDay,
            PlcDataType.DtLdt => EDataType.LDT,
            PlcDataType.DtDtl => EDataType.DTL,
            PlcDataType.DtIecLtimer => EDataType.IEC_LTimer,
            PlcDataType.DtIecScounter => EDataType.IEC_SCounter,
            PlcDataType.DtIecDcounter => EDataType.IEC_DCounter,
            PlcDataType.DtIecLcounter => EDataType.IEC_LCounter,
            PlcDataType.DtIecUcounter => EDataType.IEC_UCounter,
            PlcDataType.DtIecUdcounter => EDataType.IEC_UDCounter,
            PlcDataType.DtIecUscounter => EDataType.IEC_USCounter,
            PlcDataType.DtIecUlcounter => EDataType.IEC_ULCounter,
            PlcDataType.DtErrorStruct => EDataType.ErrorStruct,
            PlcDataType.DtNref => EDataType.NREF,
            PlcDataType.DtCref => EDataType.CREF,
            PlcDataType.DtAomIdent => EDataType.Aom_Ident,
            PlcDataType.DtEventAny => EDataType.Event_Any,
            PlcDataType.DtEventAtt => EDataType.Event_Att,
            PlcDataType.DtEventHwInt => EDataType.Event_HwInt,
            PlcDataType.DtHwAny => EDataType.Hw_Any,
            PlcDataType.DtHwIoSystem => EDataType.Hw_IoSystem,
            PlcDataType.DtHwDpMaster => EDataType.Hw_DpMaster,
            PlcDataType.DtHwDevice => EDataType.Hw_Device,
            PlcDataType.DtHwDpSlave => EDataType.Hw_DpSlave,
            PlcDataType.DtHwIo => EDataType.Hw_Io,
            PlcDataType.DtHwModule => EDataType.Hw_Module,
            PlcDataType.DtHwSubModule => EDataType.Hw_SubModule,
            PlcDataType.DtHwHsc => EDataType.Hw_Hsc,
            PlcDataType.DtHwPwm => EDataType.Hw_Pwm,
            PlcDataType.DtHwPto => EDataType.Hw_Pto,
            PlcDataType.DtHwInterface => EDataType.Hw_Interface,
            PlcDataType.DtHwIeport => EDataType.Hw_IEPort,
            PlcDataType.DtObAny => EDataType.OB_Any,
            PlcDataType.DtObDelay => EDataType.OB_Delay,
            PlcDataType.DtObTod => EDataType.OB_Tod,
            PlcDataType.DtObCyclic => EDataType.OB_Cyclic,
            PlcDataType.DtObAtt => EDataType.OB_Att,
            PlcDataType.DtConnAny => EDataType.Conn_Any,
            PlcDataType.DtConnPrg => EDataType.Conn_Prg,
            PlcDataType.DtConnOuc => EDataType.Conn_Ouc,
            PlcDataType.DtConnRId => EDataType.Conn_R_ID,
            PlcDataType.DtPort => EDataType.Port,
            PlcDataType.DtRtm => EDataType.Rtm,
            PlcDataType.DtPip => EDataType.Pip,
            PlcDataType.DtObPcycle => EDataType.OB_PCycle,
            PlcDataType.DtObHwInt => EDataType.OB_HwInt,
            PlcDataType.DtObDiag => EDataType.OB_Diag,
            PlcDataType.DtObTimeError => EDataType.OB_TimeError,
            PlcDataType.DtObStartup => EDataType.OB_Startup,
            PlcDataType.DtDbAny => EDataType.DB_Any,
            PlcDataType.DtDbWww => EDataType.DB_WWW,
            PlcDataType.DtDbDyn => EDataType.DB_Dyn,
            PlcDataType.DtDb => EDataType.DB,
            _ => EDataType.Unknown
        };
    }

    public static PrimitiveDataType GetPrimitiveDatatype(EPrimitiveDataType plcPrimitiveDatatype)
    {
        return plcPrimitiveDatatype switch
        {
            EPrimitiveDataType.Struct => PrimitiveDataType.Struct,
            EPrimitiveDataType.Bool => PrimitiveDataType.Bool,
            EPrimitiveDataType.Int8 => PrimitiveDataType.Int8,
            EPrimitiveDataType.Int16 => PrimitiveDataType.Int16,
            EPrimitiveDataType.Int32 => PrimitiveDataType.Int32,
            EPrimitiveDataType.Int64 => PrimitiveDataType.Int64,
            EPrimitiveDataType.UInt8 => PrimitiveDataType.Uint8,
            EPrimitiveDataType.UInt16 => PrimitiveDataType.Uint16,
            EPrimitiveDataType.UInt32 => PrimitiveDataType.Uint32,
            EPrimitiveDataType.UInt64 => PrimitiveDataType.Uint64,
            EPrimitiveDataType.Float => PrimitiveDataType.Float,
            EPrimitiveDataType.Double => PrimitiveDataType.Double,
            EPrimitiveDataType.Char => PrimitiveDataType.Char,
            EPrimitiveDataType.WChar => PrimitiveDataType.Wchar,
            _ => PrimitiveDataType.PdtUnspecific
        };
    }
    
    public static EPrimitiveDataType GetPrimitiveDatatype(PrimitiveDataType primitiveDataType)
    {
        return primitiveDataType switch
        {
            PrimitiveDataType.Struct => EPrimitiveDataType.Struct,
            PrimitiveDataType.Bool => EPrimitiveDataType.Bool,
            PrimitiveDataType.Int8 => EPrimitiveDataType.Int8,
            PrimitiveDataType.Int16 => EPrimitiveDataType.Int16,
            PrimitiveDataType.Int32 => EPrimitiveDataType.Int32,
            PrimitiveDataType.Int64 => EPrimitiveDataType.Int64,
            PrimitiveDataType.Uint8 => EPrimitiveDataType.UInt8,
            PrimitiveDataType.Uint16 => EPrimitiveDataType.UInt16,
            PrimitiveDataType.Uint32 => EPrimitiveDataType.UInt32,
            PrimitiveDataType.Uint64 => EPrimitiveDataType.UInt64,
            PrimitiveDataType.Float => EPrimitiveDataType.Float,
            PrimitiveDataType.Double => EPrimitiveDataType.Double,
            PrimitiveDataType.Char => EPrimitiveDataType.Char,
            PrimitiveDataType.Wchar => EPrimitiveDataType.WChar,
            _ => EPrimitiveDataType.Unspecific
        };
    }

    public static ETagListDetails GetTagListDetails(TagListDetails filter)
    {
        return filter switch
        {
            TagListDetails.Io => ETagListDetails.IO,
            TagListDetails.M => ETagListDetails.M,
            TagListDetails.Iom => ETagListDetails.IOM,
            TagListDetails.Ct => ETagListDetails.CT,
            TagListDetails.Ioct => ETagListDetails.IOCT,
            TagListDetails.Mct => ETagListDetails.MCT,
            TagListDetails.Iomct => ETagListDetails.IOMCT,
            TagListDetails.Db => ETagListDetails.DB,
            TagListDetails.Iodb => ETagListDetails.IODB,
            TagListDetails.Mdb => ETagListDetails.MDB,
            TagListDetails.Iomdb => ETagListDetails.IOMDB,
            TagListDetails.Ctdb => ETagListDetails.CTDB,
            TagListDetails.Ioctdb => ETagListDetails.IOCTDB,
            TagListDetails.Mctdb => ETagListDetails.MCTDB,
            TagListDetails.Iomctdb => ETagListDetails.IOMCTDB,
            _ => ETagListDetails.None
        };
    }

    public static TagListDetails GetTagListDetails(ETagListDetails filter)
    {
        return filter switch
        {
            ETagListDetails.IO => TagListDetails.Io,
            ETagListDetails.M => TagListDetails.M,
            ETagListDetails.IOM => TagListDetails.Iom,
            ETagListDetails.CT => TagListDetails.Ct,
            ETagListDetails.IOCT => TagListDetails.Ioct,
            ETagListDetails.MCT => TagListDetails.Mct,
            ETagListDetails.IOMCT => TagListDetails.Iomct,
            ETagListDetails.DB => TagListDetails.Db,
            ETagListDetails.IODB => TagListDetails.Iodb,
            ETagListDetails.MDB => TagListDetails.Mdb,
            ETagListDetails.IOMDB => TagListDetails.Iomdb,
            ETagListDetails.CTDB => TagListDetails.Ctdb,
            ETagListDetails.IOCTDB => TagListDetails.Ioctdb,
            ETagListDetails.MCTDB => TagListDetails.Mctdb,
            ETagListDetails.IOMCTDB => TagListDetails.Iomctdb,
            _ => TagListDetails.Unspecified
        };
    }
    
}