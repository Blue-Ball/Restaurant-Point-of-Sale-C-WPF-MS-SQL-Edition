using System.Data;


namespace RestPOS
{

  /// <summary>
  /// ///////////
  /// Author : Tuaha
  /// Country: Canada
  /// </summary>
  public static class UserInfo
  {
    public static string Userid { get; set; }
    public static string UserName { get; set; }
    public static string UserPassword { get; set; }
    public static string usertype { get; set; }
    public static string invoiceNo { get; set; }
    public static string Shopid { get; set; }
    public static string usernamWK { get; set; }
    public static string DefaulfSRWindow { get; set; }
  }

  public static class ReportValue  // use in report
  {
    public static string StartDate { get; set; }
    public static string EndDate { get; set; }
    public static string emp { get; set; }
    // public static string Reportid { get; set; }
    public static string Terminal { get; set; }
    //public static string StartDateGroupby { get; set; }
    //public static string EndDateGroupby { get; set; }
  }

  public static class parameter
  {
    public static string helpid { get; set; }
    public static string peopleid { get; set; }
    public static string footermsg { get; set; }
    public static string autoprint { get; set; }

    public static string resumesalesstatus { get; set; }
    public static string holdtransactionID { get; set; }
    public static string holdtableno { get; set; }
    public static int holdsalesid { get; set; }

    public static string currencysign
    {
      set
      {
      }
      get
      {
        string sqldis = "select currencysign from tbl_terminallocation   where shopid = '" + UserInfo.Shopid + "' ";
        DataAccess.ExecuteSQL(sqldis);
        DataTable dtdis = DataAccess.GetDataTable(sqldis);
        string csign = dtdis.Rows[0].ItemArray[0].ToString();
        return csign;
      }
    }
  }

  public static class language
  {
    public static string ID { get; set; }
    public static string languagecode { get; set; }

    public static string languagecodevalue
    {
      set
      {
      }
      get
      {
        string sqldis = "select languagecode from tbl_terminallocation   where shopid = '" + UserInfo.Shopid + "' ";
        DataAccess.ExecuteSQL(sqldis);
        DataTable dtdis = DataAccess.GetDataTable(sqldis);
        string vl = dtdis.Rows[0].ItemArray[0].ToString();
        return vl;
      }
    }
  }

  public static class vatdisvalue
  {
    public static string vat
    {
      set
      {
        //   //Load Vat and Discount rate
        //   string sqlVat= "select * from storeconfig";
        //   DataAccess.ExecuteSQL(sqlVat);
        //   DataTable dtVat = DataAccess.GetDataTable(sqlVat);
        ////   txtVATRate.Text = dtVatdis.Rows[0].ItemArray[6].ToString();
        //  // txtDiscountRate.Text = dtVatdis.Rows[0].ItemArray[7].ToString();
        //   string vl =  dtVat.Rows[0].ItemArray[6].ToString();
        //   vl = value;              
      }
      get
      {
        string sqlVat = " select VAT from tbl_terminallocation where shopid ='" + UserInfo.Shopid + "' "; // 'MTQC02' "; //
        DataAccess.ExecuteSQL(sqlVat);
        DataTable dtVat = DataAccess.GetDataTable(sqlVat);
        string vl = dtVat.Rows[0].ItemArray[0].ToString();
        return vl;
      }
    }

    public static string dis
    {
      set
      {
        //string sqldis = "select * from storeconfig";
        //DataAccess.ExecuteSQL(sqldis);
        //DataTable dtdis = DataAccess.GetDataTable(sqldis);
        //string vl = dtdis.Rows[0].ItemArray[7].ToString();
        //vl = value;
      }
      get
      {
        string sqldis = "select Dis from tbl_terminallocation   where shopid = '" + UserInfo.Shopid + "' ";
        DataAccess.ExecuteSQL(sqldis);
        DataTable dtdis = DataAccess.GetDataTable(sqldis);
        string vl = dtdis.Rows[0].ItemArray[0].ToString();
        return vl;
      }
    }

  }


}
