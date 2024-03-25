

INSERT INTO dbo.Widgets
(
    WidgetName,
    Name,
    Location,
    ModuleType,
    Description,
    ShowFooter,
    ShowFirstRow,
    GraphEnabled,
    GraphFucntion,
    HeaderStart,
    HeaderEnd,
    SkipHeaders,
    DirectRun,
    LegendStart,
    LegendEnd,
    CategoryIndex,
    SkipLegends,
    SequenceNumber,
    JsFunction,
    Template,
    IsExternal,
    IsExecuted,
    IsDeleted,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt,
    IsTileWidget,
    ReloadCondition,
    IsReloadRequired
)
VALUES
(   'UserSaleByAllDivision',        -- WidgetName - varchar(500)
    'User Sale by Division',        -- Name - varchar(250)
    'dbo.CRM_GetUserSalesByDivision_All',        -- Location - varchar(500)
    0,         -- ModuleType - tinyint
    '',        -- Description - varchar(500)
    0,      -- ShowFooter - bit
    0,      -- ShowFirstRow - bit
    1,      -- GraphEnabled - bit
    '',        -- GraphFucntion - varchar(500)
    0,         -- HeaderStart - int
    0,         -- HeaderEnd - int
    '',        -- SkipHeaders - varchar(500)
    1,      -- DirectRun - bit
    0,         -- LegendStart - int
    0,         -- LegendEnd - int
    0,         -- CategoryIndex - int
    '',        -- SkipLegends - varchar(500)
    0,         -- SequenceNumber - int
    'GetUserSaleByDivision',        -- JsFunction - varchar(50)
    '#template-crm_sale',        -- Template - varchar(500)
    0,      -- IsExternal - bit
    0,      -- IsExecuted - bit
    0,      -- IsDeleted - bit
    3245,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
    NULL, -- ModifiedAt - datetime
    0,      -- IsTileWidget - bit
    'GetVehiclePostDatedCheque_ReloadhRequired {CompanyId},''{LastLoadTime}''',        -- ReloadCondition - varchar(1000)
    1       -- IsReloadRequired - bit
    )

	DECLARE @WidgetId INT=91
	--INSERT	INTO dbo.WidgetParameters
	--(
	--    WidgetId,
	--    Name,
	--    DisplayLabel,
	--    Type,
	--    ControlType,
	--    InplutCssClass,
	--    Value,
	--    SequenceNumber,
	--    AdvanceType,
	--    IsVisible,
	--    [Key],
	--    JSFucntion,
	--    IsDeleted,
	--    CreatedBy,
	--    CreatedAt,
	--    ModifiedBy,
	--    ModifiedAt
	--)
	--VALUES
	--(   @WidgetId,         -- WidgetId - int
	--    '@CompanyId',        -- Name - varchar(50)
	--    'Company Id',        -- DisplayLabel - varchar(50)
	--    'Int',        -- Type - varchar(50)
	--    'text',        -- ControlType - varchar(500)
	--    '',        -- InplutCssClass - varchar(50)
	--    '',        -- Value - varchar(100)
	--    1,         -- SequenceNumber - int
	--    1,         -- AdvanceType - tinyint
	--    0,      -- IsVisible - bit
	--    'CompanyId',        -- Key - varchar(500)
	--    '',        -- JSFucntion - varchar(500)
	--    0,      -- IsDeleted - bit
	--    0,         -- CreatedBy - int
	--    GETDATE(), -- CreatedAt - datetime
	--    0,         -- ModifiedBy - int
	--    GETDATE()  -- ModifiedAt - datetime
	--    )

	--	INSERT	INTO dbo.WidgetParameters
	--(
	--    WidgetId,
	--    Name,
	--    DisplayLabel,
	--    Type,
	--    ControlType,
	--    InplutCssClass,
	--    Value,
	--    SequenceNumber,
	--    AdvanceType,
	--    IsVisible,
	--    [Key],
	--    JSFucntion,
	--    IsDeleted,
	--    CreatedBy,
	--    CreatedAt,
	--    ModifiedBy,
	--    ModifiedAt
	--)
	--VALUES
	--(   @WidgetId,         -- WidgetId - int
	--    '@UserId',        -- Name - varchar(50)
	--    'User Id',        -- DisplayLabel - varchar(50)
	--    'Int',        -- Type - varchar(50)
	--    'text',        -- ControlType - varchar(500)
	--    '',        -- InplutCssClass - varchar(50)
	--    '',        -- Value - varchar(100)
	--    2,         -- SequenceNumber - int
	--    11,         -- AdvanceType - tinyint
	--    0,      -- IsVisible - bit
	--    '',        -- Key - varchar(500)
	--    '',        -- JSFucntion - varchar(500)
	--    0,      -- IsDeleted - bit
	--    0,         -- CreatedBy - int
	--    GETDATE(), -- CreatedAt - datetime
	--    0,         -- ModifiedBy - int
	--    NULL  -- ModifiedAt - datetime
	--    )

	--	INSERT	INTO dbo.WidgetParameters
	--(
	--    WidgetId,
	--    Name,
	--    DisplayLabel,
	--    Type,
	--    ControlType,
	--    InplutCssClass,
	--    Value,
	--    SequenceNumber,
	--    AdvanceType,
	--    IsVisible,
	--    [Key],
	--    JSFucntion,
	--    IsDeleted,
	--    CreatedBy,
	--    CreatedAt,
	--    ModifiedBy,
	--    ModifiedAt
	--)
	--VALUES
	--(   @WidgetId,         -- WidgetId - int
	--    '@FiscalIds',        -- Name - varchar(50)
	--    'Fiscal Ids',        -- DisplayLabel - varchar(50)
	--    'string',        -- Type - varchar(50)
	--    'text',        -- ControlType - varchar(500)
	--    '',        -- InplutCssClass - varchar(50)
	--    '',        -- Value - varchar(100)
	--    3,         -- SequenceNumber - int
	--    12,         -- AdvanceType - tinyint
	--    0,      -- IsVisible - bit
	--    '',        -- Key - varchar(500)
	--    '',        -- JSFucntion - varchar(500)
	--    0,      -- IsDeleted - bit
	--    0,         -- CreatedBy - int
	--    GETDATE(), -- CreatedAt - datetime
	--    0,         -- ModifiedBy - int
	--    NULL  -- ModifiedAt - datetime
	--    )


	INSERT INTO dbo.WidgetColumns
	(
	    WidgetId,
	    Name,
	    HeaderText,
	    ShowSum,
	    FooterText,
	    Colspan,
	    IsDeleted,
	    CreatedBy,
	    CreatedAt,
	    ModifiedBy,
	    ModifiedAt,
	    OldId,
	    FooterFormatingType,
	    BodyFormatingType,
	    Precision
	)
	VALUES
	(   0,         -- WidgetId - int
	    '',        -- Name - varchar(50)
	    '',        -- HeaderText - varchar(50)
	    NULL,      -- ShowSum - bit
	    '',        -- FooterText - varchar(50)
	    0,         -- Colspan - int
	    NULL,      -- IsDeleted - bit
	    0,         -- CreatedBy - int
	    GETDATE(), -- CreatedAt - datetime
	    0,         -- ModifiedBy - int
	    GETDATE(), -- ModifiedAt - datetime
	    0,         -- OldId - int
	    0,         -- FooterFormatingType - tinyint
	    0,         -- BodyFormatingType - tinyint
	    0          -- Precision - int
	    )