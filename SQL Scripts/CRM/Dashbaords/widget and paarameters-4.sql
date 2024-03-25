USE amex_dev

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
(   'revenue-targeted-cs-achieved',        -- WidgetName - varchar(500)
    'Revenue By User',        -- Name - varchar(250)
    'dbo.CRM_GetUserTargetAndAchieved',        -- Location - varchar(500)
    0,         -- ModuleType - tinyint
    '',        -- Description - varchar(500)
    0,      -- ShowFooter - bit
    0,      -- ShowFirstRow - bit
    1,      -- GraphEnabled - bit
    '',        -- GraphFucntion - varchar(500)
    0,         -- HeaderStart - int
    2,         -- HeaderEnd - int
    '',        -- SkipHeaders - varchar(500)
    1,      -- DirectRun - bit
    1,         -- LegendStart - int
    2,         -- LegendEnd - int
    0,         -- CategoryIndex - int
    '',        -- SkipLegends - varchar(500)
    3,         -- SequenceNumber - int
    '',        -- JsFunction - varchar(50)
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

	DECLARE @WidgetId INT=94
	DECLARE @SelectWidgetId INT=93
	INSERT	INTO dbo.WidgetParameters
	(
	    WidgetId,
	    Name,
	    DisplayLabel,
	    Type,
	    ControlType,
	    InplutCssClass,
	    Value,
	    SequenceNumber,
	    AdvanceType,
	    IsVisible,
	    [Key],
	    JSFucntion,
	    IsDeleted,
	    CreatedBy,
	    CreatedAt,
	    ModifiedBy,
	    ModifiedAt
	)
	
	SELECT 
           @WidgetId,
           Name,
           DisplayLabel,
           Type,
           ControlType,
           InplutCssClass,
           Value,
           SequenceNumber,
           AdvanceType,
           IsVisible,
           [Key],
           JSFucntion,
           IsDeleted,
           CreatedBy,
           CreatedAt,
           ModifiedBy,
           ModifiedAt FROM dbo.WidgetParameters
	WHERE WidgetId=@SelectWidgetId
	