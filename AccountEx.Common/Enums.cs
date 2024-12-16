
namespace AccountEx.Common
{
    public enum RecordStatus
    {
        Active = 0,
        Inactive = 1,
        Deleted = 2,
        Blocked = 3,
    }
    public enum OpeningBalances
    {
        Stock = 1,
    }
    public enum SalaryStatus
    {
        UnApproved = 0,
        Approved = 1,
        EmailToBank = 3,
        BankProcessed = 4,
    }
    public enum TransactionStatus
    {
        Pending = 0,
        Dispatch = 1,
        Invoiced = 2,
        Blocked = 3,
        Delivered = 4,
        PartialyDelivered = 5,
        Ready = 7,
        FGRN = 8,
        PendingProduction = 11,

    }
    public enum ReportParameterType
    {
        Simple = 0,
        Session = 1,
        Settings = 2,
        FiscalYearStartDate = 3,
        DefaultValue = 4,
        FiscalYearEndDate = 5,
        FiscalId = 6,
        IdType = 7,
        MonthStartDate = 8,
        MonthEndDate = 9,
        TodayDate = 10,
        LoggedInUserId = 11,
        CurenntFiscalWithPrevious = 12,

    }

    public enum EntryType
    {
        MasterDetail = 1,
        Item = 2,
        Discount = 3,
        Gst = 4,
        Wht = 5,
        IncomeTax = 6,
        MedicalAllownce = 7,
        HouseRent = 8,
        EtcAllownce = 9,
        OverTime = 10,
        Installement = 11,
        Eobi = 12,
        ProvidentFund = 13,
        Insurance = 14,
        ConvyenceAllownce = 15,
        SocialSecurity = 16,
        RawMaterial = 17,
        FinishedGoods = 18,
        HeadAccount = 19,

        ServiceExpense = 20,
        Promotion = 21,

        Manual = 22,
        Automatic = 23,
        MonthlyRent = 24,
        UCPercent = 25,
        ElectricityCharges = 26,
        Receiving = 27,
        Surcharge = 28,
        Possession = 29,
        Security = 30,
        Misc = 31,
        Charges = 32,
        ServicesSupplier = 33,
        VehicleSupplier = 34,
        Services = 35,
        TrackerPurchase = 36,
        TrackerSale = 37,
        InsuranceSale = 38,
        Depriciation = 39,
        CGS = 40,
        InventoryConsumpation = 41,
        Labours = 42,


    }
    public enum VoucherType : byte
    {
        None = 0,
        Sale = 1,
        Purchase = 2,
        SaleReturn = 3,
        PurchaseReturn = 4,
        CashReceipts = 5,
        CashPayments = 6,
        BankReceipts = 7,
        BankPayments = 8,
        StoreTransaction = 9,
        OpeningBalance = 10,
        Recovery = 11,
        TransferVoucher = 12,
        Salary = 13,
        Project = 14,
        Production = 15,
        Adjustment = 16,
        Services = 17,
        SaleOrder = 18,
        PurchaseOrder = 19,
        GoodIssue = 20,
        GoodReceive = 21,
        GINP = 22,
        FGRN = 23,
        Requisition = 24,
        GstSale = 25,
        GstPurchase = 26,
        GstSaleReturn = 27,
        GstPurchaseReturn = 28,
        AutoOpeningBalance = 29,
        AutoClosingBalance = 30,
        SaleDiscounts = 31,
        CustomerServiceOrder = 32,
        SiteServiceOrder = 33,
        RepairingServiceOrder = 34,
        AdjustmentIn = 35,
        AdjustmentOut = 36,
        PossessionCharges = 37,
        SecurityMoney = 38,
        MiscCharges = 39,
        RentAgreement = 40,
        FortressBankReceipt = 41,
        RentMonthlyLiability = 42,
        vehiclecashsale = 43,
        vehicleinstallmentsale = 44,
        VCR = 45,
        VBR = 46,
        BL = 47,
        RC = 48,
        Penalty = 49,
        TradeIn = 50,
        ROB = 51,
        BLPayment = 52,
        AuctionnerCharges = 53,
        LocalPurchase = 54,
        VehiclePayable = 55,
        PayablePayment = 56,
        VSD = 57,
        AdvanceReceipts = 58,
        PenaltyPayments = 59,
        AuctionnerPayments = 60,
        BLPurchase = 61,
        ElectictyChallan = 62,
        ForexVoucher = 63,
        ActiveStock = 64,
        PostedCases = 65,
        VatST = 66,
        Breakage = 67,
        WheatPurchase = 68,
        ProductionBirds = 69,
        ProductionFeedReceive = 70,
        ProductionFinishGood = 71,
        AutoBudget = 72,
        Labours=73,
        CarRent=74,
        Doctor =75,
    }


    public enum AccountDetailFormType
    {
        None = 0,
        Assets = 1,
        Banks = 2,
        Customers = 3,
        Employees = 4,
        Products = 5,
        Suppliers = 6,
        Salesman = 7,
        Services = 8,
        Equipments = 9,
        OrderTakers = 10,
        TerritoryManagers = 11,
        Tenant = 12,
        Expences = 13,
        Duties = 14,
        Labours = 15,
    }

    public enum ActivityType
    {
        Task = 1,
        Event = 2,
        Call = 3,
    };
    public enum SaleTypeEnum
    {
        CreditSale = 1,
        CashSale = 2,
        Sampling = 3,
    };

    public enum BardanaType
    {
        Plastic = 1,
        Jute = 2,
    }
    public enum BehaviourLevel
    {
        Excellen = 1,
        Good = 2,
        Satisfactory = 3,
        Poor = 4,
        VeryPoor = 5

    }
    public enum LogType
    {
        Default = 0,
        Transaction = 1,
        Service = 2,
        ChartOfAccount = 3,

    }
    public enum ModuleType
    {
        Default = 0,
        Sale = 1,
        Purchase = 2,
        ChartOfAccount = 3,

    }
    public enum ActionType
    {
        Added = 1,
        Updated = 2,
        Deleted = 3,

    }
    public enum LeaveCategory
    {
        Annual = 1,
        Sick = 2,
        Casual = 3,
        Absent = 4,
    }
    public enum OrderType
    {
        FinishedGoods = 1,
        Production = 2,
        Services = 3,
    }
    public enum FormatingType
    {
        Number = 1,
        Float = 2,
        NumberWithThousandSeprator = 3,
        FloatWithThousandSeprator = 4,
        ThousandSeprator = 5,
    }
    public enum GroupType
    {
        ItemGroup = 1,
        CustomerIncentiveGroup = 2,
    }
    public enum GroupSubType
    {
        General = 1,
        Promotion = 2,
        Less = 3,
    }
    public enum GrapyType
    {
        Default = 0,
        PieChart = 1,
    }

    public enum NotificationActions
    {
        Add = 1,
        Modify = 2,
        Delete = 3,
        Finalized = 4

    }

    public enum NotificationJobTypes
    {
        //Trigger on specified actions
        ActionBased = 1,
        //Trigger on scheduled time
        Scheduled = 2,
        //Trigger on reaching Min/Max Limits
        Threshold = 3
    }

    public enum VehicleRequestType
    {
        Send = 1,
        Recieve = 2,
    }
    public enum VehicleRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
    }
    public enum RecoveryStatus
    {
        Default = 0,
        InProcess = 1,
        Recovered = 2,
        InventoryReturn = 3,
        CustomerReturn = 4,
        Advertisement = 5,
        PrintPossession = 6,
        NotficationLetter = 7,
        PrintNotficationLetter = 8,
        FinalAuctionnerCharges = 9,
        PrintFurtherAgreement = 10,
        SaleReturn = 11
    }
    public enum VehicleStatus
    {
        Default = 0,
        Sale = 1,
        SaleReturn = 2,
        PurchaseReturn = 3
    }
    public enum LogBookStatus
    {
        Default = 0,
        Apply = 1,
        Received = 2,
        Transferred = 3,
    }
    public enum VoucherAccountType
    {
        Fixed = 1,
        Multiple = 2,
    }
    public enum AdjustmentType
    {
        Less = 1,
        Increase = 2,
    }
    public enum VehicleType
    {
        New = 1,
        TradIn = 2,
        LocalPurchase = 3,
    }
    public enum AgreementStatus
    {
        Default = 0,
        Renew = 1,
        Transfeer = 2,
        Close = 4,
        InActive = 5,
    }
    public enum VehiclePostDatedChequeType
    {
        Dishonour = 1,
        Clear = 2,
        PaidCash = 3,
        Hold = 4,
        PresentAndWaitingToClear = 5,
    }
    public enum CRMUserType : byte
    {
        CEO = 1,
        Admin = 2,
        DivisionalHead = 3,
        RSM = 4,
        SalesExecutive = 5,
        Client = 6,
        LabUser = 7,
    }
    public enum CRMCustomerType : byte
    {
        All = 0,
        Customer = 1,
        Trader = 2,
    }
    public enum TestType
    {
        Pathology = 1,
        Radiology = 2,


    }
    public enum NexusCaseType
    {
        Cash = 1,
        Departmental = 2,


    }
    public enum CRMImportRequisitionStatus : byte
    {
        Pending = 0,
        Approved = 1,
        Review = 2,
        Revised = 3,
        Archive = 4,


    }
    public enum CRMImportRequisitionType
    {
        Default = 0,
        DH = 1,
        RSM = 2,


    }
    public enum CRMSaleForecastType
    {
        SalePerson = 0,
        RSM = 1,
        Summary = 2,


    }
    public enum CRMSaleDeliveryType : byte
    {
        Default = 0,
        ExStock = 1,
        Import = 2,


    }
    public enum CRMJRFType
    {
        Default = 0,
        DH = 1,
        RSM = 2,


    }
    public enum CRMJRFStatus : byte
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,



    }
    public enum CRMComplaintStatus : byte
    {
        New = 0,
        Assigned = 1,
        Open = 2,
        WaitingForCustomer = 3,
        Resolved = 4,
        Closed = 5,



    }
    public enum CRMInvoiceType : byte
    {
        Default = 0,
        GST = 1,
        NonGST = 2,


    }
    public enum CRMSaleType : byte
    {
        Project = 1,
        Reguler = 2,


    }
    public enum CRMVendorType : byte
    {
        Default = 0,
        Import = 1,

    }
    public enum AuthenticationType
    {
        facebook = 1,
        linkedin = 2,
        googlecalendar = 3,
        twitter = 4,
        viadeo = 5,
        xing = 6,
        hotmail = 7

    }
    public enum TokenType : byte
    {
        Access = 1,
        Refresh = 2,
    }
    public enum CRMLabCaseType : byte
    {
        ProjectTesting = 1,
        OwnProductTesting = 2,
        CompProductTesting = 3,

    }
    public enum ProductionType : byte
    {
        Production = 0,
        Breakage = 1,

    }
    public enum TagType : byte
    {
        Sale = 1,
        SaleItem = 2,

    }
    public enum StockType : byte
    {
        Active = 1,
        Waste = 2,
        Consume = 3

    }
    public enum StockTransferType : byte
    {
        LocationWise = 1,
        MachineWise = 2,
        Production = 3,

    }
    public enum ProductionStatus: byte
    {
        None = 0,
        StockRequested = 1,
        StockIssued = 2,
        StockReceived = 3,
        ProductionStarted = 4,
        ProductionCompleted = 5
    }
    public enum ProductionUnitItemType : byte
    {
        Raw = 1,
        Finished = 2,
    }
}