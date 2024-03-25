var EntryType =
{
    MasterDetail: 1,
    Item: 2,
    Discount: 3,
    Gst: 4,
    Wht: 5,
    IncomeTax: 6,
    MedicalAllownce: 7,
    HouseRent: 8,
    EtcAllownce: 9,
    OverTime: 10,
    Installement: 11,
    Eobi: 12,
    ProvidentFund: 13,
    Insurance: 14,
    ConvyenceAllownce: 15,
    SocialSecurity: 16,
    RawMaterial: 17,
    FinishedGoods: 18,
    HeadAccount: 19,

    ServiceExpense: 20,
    Promotion: 21,

    Manual: 22,
    Automatic: 23,
    MonthlyRent: 24,
    UCPercent: 25,
    ElectricityCharges: 26,
    Receiving: 27,
    Surcharge: 28,
    Possession: 29,
    Security: 30,
    Misc: 31,
    Charges: 32,
    ServicesSupplier: 33,
    VehicleSupplier: 34,
    Services: 35,

};
var TransactionStatus =
{
    PendingOrder: 0,
    Dispatch: 1,
    Invoiced: 2,
    Blocked: 3,
    Delivered: 4,
    PartialyDelivered: 5,
    PendingDc: 6,
    PendingProduction: 11,
};
var GroupType =
    {
        itemgroup: 1,
        customerincentivegroup: 2,
    };
var GroupSubType =
    {
        general: 1,
        promotion: 2,
        less: 3,
    };
var VoucherType =
{
    sale: 1,
    purchase: 2,
    salereturn: 3,
    purchasereturn: 4,
    cashreceipts: 5,
    cashpayments: 6,
    bankreceipts: 7,
    bankpayments: 8,
    storetransaction: 9,
    openingbalance: 10,
    recovery: 11,
    transfervoucher: 12,
    production: 15,
    adjustment: 16,
    services: 17,
    saleorder: 18,
    purchaseorder: 19,
    goodissue: 20,
    goodreceive: 21,
    ginp: 22,
    fgrn: 23,
    requisition: 24,
    gstsale: 25,
    gstpurchase: 26,
    gstsalereturn: 27,
    gstpurchasereturn: 28,
    autoopeningbalance: 29,
    autoclosingbalance: 30,
    salediscounts: 31,
    customerserviceorder: 32,
    siteserviceorder: 33,
    repairingserviceorder: 34,
    possessioncharges: 37,
    securitymoney: 38,
    misccharges: 39,
    rentagreement: 40,
    RentMonthlyLiability: 42,
    vehiclecashsale: 43,
    vehicleinstallmentsale: 44,
    vcr: 45,
    vbr: 46,
    bl: 47,
    RC: 48,
    penalty: 49,
    tradeIn: 50,
    rob: 51,
    blpayment: 52,
    auctionnercharges: 53,
    localpurchase: 54,
    vehiclepayable: 55,
    payablepayment: 56,
    VSD: 57,
    advancereceipts: 58,
    penaltypayments: 59,
    auctionnerpayments: 60,
    blpurchase: 61,
    electictychallan: 62,
    forexvoucher: 63,
    activestock: 64,
    postedcases: 65,
    vatst: 66,
    breakage: 67,
    wheatpurchase: 68,
    labours: 73,
    doctor:75,
};

var VoucherShortName =
{
    SV: 1,
    PV: 2,
    SR: 3,
    PR: 4,
    CR: 5,
    CP: 6,
    BR: 7,
    BP: 8,
    JV: 12,
    ATI: 35,
    ATO: 36,
};
//var EntryType =
//{
//    Manual: 22,
//    Automatic: 23,
//};
var SaleType =
    {
        creditsale: 1,
        cashsale: 2,
        sampling: 3,
    };
var OrderType =
{
    FinishedGoods: 1,
    Production: 2,
};
var ActivityType =
{
    Task: 1,
    Event: 2,
    Call: 3,
};
var VoucherTypes = new Array();
VoucherTypes[0] = { Code: "NA", Description: "N/A" };
VoucherTypes[1] = { Code: "SV", Description: "Sale Voucher" };
VoucherTypes[2] = { Code: "PV", Description: "Purchase Voucher" };
VoucherTypes[3] = { Code: "SRV", Description: "Sale Return Voucher" };
VoucherTypes[4] = { Code: "PRV", Description: "Purchase Return Voucher" };
VoucherTypes[5] = { Code: "CRV", Description: "Cash Receipt Voucher" };
VoucherTypes[6] = { Code: "CPV", Description: "Cash Payment Voucher" };
VoucherTypes[7] = { Code: "BRV", Description: "Bank Receipt Voucher" };
VoucherTypes[8] = { Code: "BPV", Description: "Bank Payment Voucher" };
VoucherTypes[9] = { Code: "ST", Description: "StoreTransaction" };
VoucherTypes[10] = { Code: "OB", Description: "Opening Balance" };
VoucherTypes[11] = { Code: "RC", Description: "Recovery" };
VoucherTypes[12] = { Code: "JV", Description: "Journal Voucher" };
VoucherTypes[13] = { Code: "SL", Description: "Salary" };
VoucherTypes[14] = { Code: "PR", Description: "Project" };
VoucherTypes[15] = { Code: "PrV", Description: "Production" };
VoucherTypes[16] = { Code: "AdV", Description: "Adjustment" };
VoucherTypes[17] = { Code: "SrV", Description: "Services Voucher" };
VoucherTypes[18] = { Code: "SO", Description: "Sale Order" };
VoucherTypes[19] = { Code: "PO", Description: "Purchase Order" };
VoucherTypes[20] = { Code: "GIN", Description: "Good Issue Note" };
VoucherTypes[21] = { Code: "GRN", Description: "Good Receieve Note" };
VoucherTypes[22] = { Code: "GINP", Description: "Good Issue Note for Production" };
VoucherTypes[23] = { Code: "FGRN", Description: "Finish Good Receieve Note" };
VoucherTypes[24] = { Code: "REQ", Description: "Requisition" };
VoucherTypes[25] = { Code: "GSTSV", Description: "GST Sale Voucher" };
VoucherTypes[26] = { Code: "GSTPV", Description: "GST Purchase Voucher" };
VoucherTypes[27] = { Code: "GSTSRV", Description: "GST Sale Return Voucher" };
VoucherTypes[28] = { Code: "GSTPRV", Description: "GST Purchase Return Voucher" };
VoucherTypes[29] = { Code: "AOB", Description: "Auto Opening Balance" };
VoucherTypes[30] = { Code: "ACB", Description: "Auto Closing Balance" };
VoucherTypes[35] = { Code: "ATI", Description: "Adjustment In" };
VoucherTypes[36] = { Code: "ATO", Description: "Adjustment Out" };
VoucherTypes[37] = { Code: "PC", Description: "Possession Charges" };
VoucherTypes[38] = { Code: "SM", Description: "Security Money" };
VoucherTypes[39] = { Code: "MC", Description: "Misc Charges" };
VoucherTypes[40] = { Code: "RA", Description: "Rent Agreement" };
VoucherTypes[41] = { Code: "FBR", Description: "Fortress Bank Receipt" };
VoucherTypes[42] = { Code: "RML", Description: "Monthly Rent Liability " };
VoucherTypes[43] = { Code: "VCS", Description: "Vehicle Cash Sale" };
VoucherTypes[44] = { Code: "VIS", Description: "Vehicle Installment Sale" };

VoucherTypes[45] = { Code: "VCR", Description: "Vehicle Cash Reciept" };
VoucherTypes[46] = { Code: "VBR", Description: "Vehicle Bank Reciept" };
VoucherTypes[47] = { Code: "BL", Description: "BL" };
VoucherTypes[48] = { Code: "RC", Description: "Rent Challan" };
VoucherTypes[49] = { Code: "PC", Description: "Penalty Charges" };
VoucherTypes[50] = { Code: "TI", Description: "Trade In" };
VoucherTypes[51] = { Code: "ROB", Description: "Rent Opening Balance" };
VoucherTypes[52] = { Code: "BLP", Description: "BL Payments" };
VoucherTypes[53] = { Code: "AC", Description: "Auctionner Charges" };
VoucherTypes[54] = { Code: "LP", Description: "Local Purchase" };
VoucherTypes[55] = { Code: "SP", Description: "Supplier Payable" };
VoucherTypes[56] = { Code: "PP", Description: "Payable Payments" };
VoucherTypes[57] = { Code: "VSD", Description: "Sale Deposit" };
VoucherTypes[58] = { Code: "AR", Description: "Advance Receipts" };
VoucherTypes[59] = { Code: "PR", Description: "Penalty Receipts" };
VoucherTypes[60] = { Code: "AR", Description: "Auctionner Receipts" };
VoucherTypes[61] = { Code: "BLP", Description: "BL Purchase" };
VoucherTypes[63] = { Code: "FP", Description: "Forex Payment" };

VoucherTypes[73] = { Code: "WEV", Description: "Wages Entry Voucher" };

VoucherTypes[74] = { Code: "DEV", Description: "Doctor Entry Voucher" };


VoucherTypes[68] = { Code: "WP", Description: "Wheat Purchase" };


var BardanaType =
{
    P: 1,
    J: 2,
};

var BehaviourLevel =
    {
        Excellent: 1,
        Good: 2,
        Satisfactory: 3,
        Poor: 4,
        VeryPoor: 5

    }
var LeaveTypes =
    {
        Annual: 1,
        Sick: 2,
        Casual: 3,
        Absent: 4,
    }
var FormatingType =
{
    Number: 1,
    Float: 2,
    NumberWithThousandSeprator: 3,
    FloatWithThousandSeprator: 4,
    ThousandSeprator: 5,
}

var GraphType =
{
    Default: 0,
    PieChart: 1,
}
var VehicleRequestType =
{
    Send: 1,
    Recieve: 2,
}

var RecoveryStatus =
{
    Default: 0,
    InProcess: 1,
    Recovered: 2,
    InventoryReturn: 3,
    CustomerReturn: 4,
    Advertisement: 5,
    PrintPossession: 6,
    NotficationLetter: 7,
    PrintNotficationLetter: 8,
    FinalAuctionnerCharges: 9,
    PrintFurtherAgreement: 10,
    SaleReturn: 11,
}
var VoucherAccountType =
{
    Fixed: 1,
    Multiple: 2,
}
var AdjustmentType =
{
    Less: 1,
    Increase: 2,
}

var LogBookStatus =
{
    Default: 0,
    Apply: 1,
    Received: 2,
    Transferred: 3
}
var VehicleType =
{
    New: 1,
    TradIn: 2,
    LocalPurchase: 3,
}
var VehiclePostDatedChequeType =
{
    Dishonour: 1,
    Clear: 2,
    PaidCash: 3,
    Hold: 4,
    PresentAndWaitingToClear: 5,
}
var AmountInWordType =
{
    Lakh: 1,
    Million: 2,
}

var CRMUserType =
{
    CEO: 1,
    Admin: 2,
    DivisionalHead: 3,
    RSM: 4,
    SalesExecutive: 5,
    Client: 6,
}
var CRMImportRequisitionStatus =
{
    Pending: 0,
    Approved: 1,
    Review: 2,
    Revised: 3,
    Archive: 4,


}
var CRMImportRequisitionType =
{
    Default: 0,
    DH: 1,
    RSM: 2,
}
var ActionType =
{
    Delete: 0,
    Senstitive: 1,
}
var CRMSaleForecastType =
{
    SalePerson: 0,
    RSM: 1,
    Summary: 2,
}
var CRMUserType =
{
    CEO: 1,
    Admin: 2,
    DivisionalHead: 3,
    RSM: 4,
    SalesExecutive: 5,
    Client: 6,
    LabUser: 7,
}
var CRMComplaintStatus =
{
    New: 0,
    Assigned: 1,
    Open: 2,
    WaitingForCustomer: 3,
    Resolved: 4,
    Closed: 5,



}
var CRMSaleDeliveryType =
{
    Default: 0,
    ExStock: 1,
    Import: 2,

}
var CRMSaleType =
{
    Default: 0,
    GST: 1,
    NonGST: 2,

}

var CRMLabCaseType =
{
    ProjectTesting: 1,
    OwnProductTesting: 2,
    CompProductTesting: 3,

}
var ProductionType =
{
    Production: 0,
    Breakage: 1,

}