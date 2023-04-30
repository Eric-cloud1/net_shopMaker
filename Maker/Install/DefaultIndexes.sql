CREATE INDEX ac_CatalogNodes_IX1 ON ac_CatalogNodes
(
	[CatalogNodeTypeId] ASC,
	[CategoryId] ASC,
	[CatalogNodeId] ASC
)
go
CREATE INDEX ac_CatalogNodes_IX2 ON ac_CatalogNodes
(
	[CategoryId] ASC,
	[CatalogNodeId] ASC
)
go
CREATE INDEX ac_PageViews_IX1 ON ac_PageViews
(
	[StoreId] ASC,
	[CatalogNodeTypeId] ASC,
	[ActivityDate] ASC
)
go
CREATE INDEX ac_Products_IX1 ON ac_Products
(
	[VisibilityId] ASC,
	[ProductId] ASC,
	[StoreId] ASC
)
go
CREATE INDEX ac_Products_IX2 ON ac_Products
(
	[StoreId] ASC,
	[VisibilityId] ASC,
	[ManufacturerId] ASC
)
go
CREATE INDEX ac_Products_IX3 ON ac_Products
(
	[ProductId] ASC,
	[VisibilityId] ASC,
	[Name] ASC
)
go
CREATE INDEX ac_Products_IX4 ON ac_Products
(
	[StoreId] ASC,
	[VisibilityId] ASC,
	[Name] ASC
)
go
CREATE INDEX  ac_OrderItems_IX1 ON ac_OrderItems
(
	[OrderId] ASC,
	[OrderBy] ASC
)