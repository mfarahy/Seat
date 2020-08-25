using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public enum FlowTypes
    {
        Common = 0,//: عمومي - مشترک بين بورس و فرابورس
        Bourse = 1,// : بورس
        FaraBourse = 2,// : فرابورس
        Ati = 3,// : آتی
        PayeFarabourse = 4,// : پایه فرابورس

    }

    public enum InstrumentStates
    {
        Unknown,
        I_Impermissible, //ممنوع 
        A_Allow,// مجاز
        AG,//مجاز-مسدود
        AS_AllowStoped,// مجاز-متوقف"
        AR_AllowReserved,// مجاز-محفوظ
        IG,// ممنوع-مسدود
        IS_ImpermissibleStoped, // ممنوع-متوقف
        IR_ImpermissibleReserved,// ممنوع-محفوظ
        
    }
}
