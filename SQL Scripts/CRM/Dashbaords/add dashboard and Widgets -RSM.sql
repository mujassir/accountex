INSERT INTO dbo.Dashboards
(
    Title,
    Name,
    IsDeleted,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt,
    ShowTiles,
    TilesSP
)
VALUES
(   'RSM',        -- Title - varchar(250)
    'RSM',        -- Name - varchar(50)
    0,      -- IsDeleted - bit
    3245,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    0,         -- ModifiedBy - int
    NULL, -- ModifiedAt - datetime
    0,      -- ShowTiles - bit
    ''         -- TilesSP - varchar(50)
    )

	SELECT * FROM dbo.Dashboards ORDER BY id DESC

	DECLARE @DashboardId INT =6

	--INSERT INTO dbo.DashboardWidgets
	--(
	--    DashboardId,
	--    WidgetId,
	--    SequenceNumber
	--)
	--VALUES
	--(   @DashboardId, -- DashboardId - int
	--    91, -- WidgetId - int
	--    1  -- SequenceNumber - int
	--    )

		INSERT INTO dbo.DashboardWidgets
	(
	    DashboardId,
	    WidgetId,
	    SequenceNumber
	)
	VALUES
	(   @DashboardId, -- DashboardId - int
	    92, -- WidgetId - int
	    1  -- SequenceNumber - int
	    )

		INSERT INTO dbo.DashboardWidgets
	(
	    DashboardId,
	    WidgetId,
	    SequenceNumber
	)
	VALUES
	(   @DashboardId, -- DashboardId - int
	    93, -- WidgetId - int
	    1  -- SequenceNumber - int
	    )

	
		INSERT INTO dbo.DashboardWidgets
	(
	    DashboardId,
	    WidgetId,
	    SequenceNumber
	)
	VALUES
	(   @DashboardId, -- DashboardId - int
	    94, -- WidgetId - int
	    1  -- SequenceNumber - int
	    )


			INSERT INTO dbo.DashboardWidgets
	(
	    DashboardId,
	    WidgetId,
	    SequenceNumber
	)
	VALUES
	(   @DashboardId, -- DashboardId - int
	    95, -- WidgetId - int
	    5  -- SequenceNumber - int
	    )

		UPDATE dbo.Widgets SET Template='#template-crm_sale' WHERE Id=91