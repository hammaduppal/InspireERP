namespace ILibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BannerSliders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        URL = c.String(),
                        ImageURL = c.String(),
                        AddDateTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Department_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.Department_Id)
                .Index(t => t.Department_Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Country_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .Index(t => t.Country_Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerNote = c.String(),
                        LoginUsers_Id = c.Int(),
                        Person_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoginUsers", t => t.LoginUsers_Id)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .Index(t => t.LoginUsers_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.LoginUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        LoginType = c.Int(nullable: false),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        MobileNumber = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StretAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StAddress = c.String(),
                        AddressType = c.Int(nullable: false),
                        City_Id = c.Int(),
                        Person_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .Index(t => t.City_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        URL = c.String(),
                        Caption = c.String(),
                        Priority = c.String(),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderMasters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDateTime = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        GrandTotal = c.Int(nullable: false),
                        Qty = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        DeliveryCharges = c.Int(nullable: false),
                        Total = c.Int(nullable: false),
                        OrderNote = c.String(),
                        Customer_Id = c.Int(),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        RetailPrice = c.Int(nullable: false),
                        CostPrice = c.Int(nullable: false),
                        DiscountPrice = c.Int(nullable: false),
                        SalePrice = c.Int(nullable: false),
                        DeliveryCharges = c.Int(nullable: false),
                        ModelNumber = c.String(),
                        SerialNumber = c.String(),
                        Qty = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        LastSold = c.DateTime(nullable: false),
                        LastPurchased = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        BarCode = c.String(),
                        IsSale = c.Boolean(nullable: false),
                        IsDiscount = c.Boolean(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                        ViewCounts = c.Int(nullable: false),
                        Brand_Id = c.Int(),
                        Color_Id = c.Int(),
                        Material_Id = c.Int(),
                        Size_Id = c.Int(),
                        SubCategory_Id = c.Int(),
                        Supplier_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.Brand_Id)
                .ForeignKey("dbo.Colors", t => t.Color_Id)
                .ForeignKey("dbo.Materials", t => t.Material_Id)
                .ForeignKey("dbo.Sizes", t => t.Size_Id)
                .ForeignKey("dbo.SubCategories", t => t.SubCategory_Id)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.Brand_Id)
                .Index(t => t.Color_Id)
                .Index(t => t.Material_Id)
                .Index(t => t.Size_Id)
                .Index(t => t.SubCategory_Id)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SCode = c.String(),
                        NTN = c.String(),
                        NTNPicture = c.String(),
                        CompanyLogo = c.String(),
                        CompanyName = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateEdited = c.DateTime(nullable: false),
                        LoginUser_Id = c.Int(),
                        Person_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoginUsers", t => t.LoginUser_Id)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .Index(t => t.LoginUser_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.PaidPromotions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Url = c.String(),
                        ImageUrl = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        AddVavlue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResetPasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        HashCode = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        RequestedTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteConfigurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tabtags = c.String(),
                        Twitter = c.String(),
                        FaceBook = c.String(),
                        WebSite = c.String(),
                        CompanyAddress = c.String(),
                        CompanyCity = c.String(),
                        CompanyCountry = c.String(),
                        CompanyMobile = c.String(),
                        CompanyEmail = c.String(),
                        CompanySkype = c.String(),
                        CompanyNote = c.String(),
                        CompanyFirstName = c.String(),
                        CompanyLastName = c.String(),
                        CompanyLogo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupplierConfigs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SupplierCreatedOn = c.DateTime(nullable: false),
                        BulkUploadFilePath = c.String(),
                        NewfileTime = c.DateTime(nullable: false),
                        Supplier_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.Supplier_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplierConfigs", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.OrderMasters", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Products", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Suppliers", "Person_Id", "dbo.People");
            DropForeignKey("dbo.Suppliers", "LoginUser_Id", "dbo.LoginUsers");
            DropForeignKey("dbo.Products", "SubCategory_Id", "dbo.SubCategories");
            DropForeignKey("dbo.SubCategories", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Products", "Size_Id", "dbo.Sizes");
            DropForeignKey("dbo.Products", "Material_Id", "dbo.Materials");
            DropForeignKey("dbo.Images", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Products", "Color_Id", "dbo.Colors");
            DropForeignKey("dbo.Products", "Brand_Id", "dbo.Brands");
            DropForeignKey("dbo.OrderMasters", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.Customers", "Person_Id", "dbo.People");
            DropForeignKey("dbo.StretAddresses", "Person_Id", "dbo.People");
            DropForeignKey("dbo.StretAddresses", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.Customers", "LoginUsers_Id", "dbo.LoginUsers");
            DropForeignKey("dbo.Cities", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.Categories", "Department_Id", "dbo.Departments");
            DropIndex("dbo.SupplierConfigs", new[] { "Supplier_Id" });
            DropIndex("dbo.Suppliers", new[] { "Person_Id" });
            DropIndex("dbo.Suppliers", new[] { "LoginUser_Id" });
            DropIndex("dbo.SubCategories", new[] { "Category_Id" });
            DropIndex("dbo.Products", new[] { "Supplier_Id" });
            DropIndex("dbo.Products", new[] { "SubCategory_Id" });
            DropIndex("dbo.Products", new[] { "Size_Id" });
            DropIndex("dbo.Products", new[] { "Material_Id" });
            DropIndex("dbo.Products", new[] { "Color_Id" });
            DropIndex("dbo.Products", new[] { "Brand_Id" });
            DropIndex("dbo.OrderMasters", new[] { "Product_Id" });
            DropIndex("dbo.OrderMasters", new[] { "Customer_Id" });
            DropIndex("dbo.Images", new[] { "Product_Id" });
            DropIndex("dbo.StretAddresses", new[] { "Person_Id" });
            DropIndex("dbo.StretAddresses", new[] { "City_Id" });
            DropIndex("dbo.Customers", new[] { "Person_Id" });
            DropIndex("dbo.Customers", new[] { "LoginUsers_Id" });
            DropIndex("dbo.Cities", new[] { "Country_Id" });
            DropIndex("dbo.Categories", new[] { "Department_Id" });
            DropTable("dbo.SupplierConfigs");
            DropTable("dbo.SiteConfigurations");
            DropTable("dbo.ResetPasses");
            DropTable("dbo.PaidPromotions");
            DropTable("dbo.Suppliers");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Sizes");
            DropTable("dbo.Products");
            DropTable("dbo.OrderMasters");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Materials");
            DropTable("dbo.Images");
            DropTable("dbo.StretAddresses");
            DropTable("dbo.People");
            DropTable("dbo.LoginUsers");
            DropTable("dbo.Customers");
            DropTable("dbo.Colors");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
            DropTable("dbo.Departments");
            DropTable("dbo.Categories");
            DropTable("dbo.Brands");
            DropTable("dbo.BannerSliders");
        }
    }
}
