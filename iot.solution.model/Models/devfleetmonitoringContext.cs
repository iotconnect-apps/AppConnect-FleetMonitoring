using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace iot.solution.model.Models
{
    public partial class devfleetmonitoringContext : DbContext
    {
        public devfleetmonitoringContext()
        {
        }

        public devfleetmonitoringContext(DbContextOptions<devfleetmonitoringContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdminRule> AdminRule { get; set; }
        public virtual DbSet<AdminUser> AdminUser { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        public virtual DbSet<AttributeValue> AttributeValue { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyConfig> CompanyConfig { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Counter> Counter { get; set; }
        public virtual DbSet<DebugInfo> DebugInfo { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceFiles> DeviceFiles { get; set; }
        public virtual DbSet<DeviceMaintenance> DeviceMaintenance { get; set; }
        public virtual DbSet<DeviceType> DeviceType { get; set; }
        public virtual DbSet<Driver> Driver { get; set; }
        public virtual DbSet<Entity> Entity { get; set; }
        public virtual DbSet<Fleet> Fleet { get; set; }
        public virtual DbSet<FleetHaltStatus> FleetHaltStatus { get; set; }
        public virtual DbSet<FleetIdleStatus> FleetIdleStatus { get; set; }
        public virtual DbSet<FleetMaterialType> FleetMaterialType { get; set; }
        public virtual DbSet<FleetType> FleetType { get; set; }
        public virtual DbSet<HardwareKit> HardwareKit { get; set; }
        public virtual DbSet<Hash> Hash { get; set; }
        public virtual DbSet<IotconnectAlert> IotconnectAlert { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobParameter> JobParameter { get; set; }
        public virtual DbSet<JobQueue> JobQueue { get; set; }
        public virtual DbSet<KitType> KitType { get; set; }
        public virtual DbSet<KitTypeAttribute> KitTypeAttribute { get; set; }
        public virtual DbSet<KitTypeCommand> KitTypeCommand { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<MasterWidget> MasterWidget { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleModulePermission> RoleModulePermission { get; set; }
        public virtual DbSet<Schema> Schema { get; set; }
        public virtual DbSet<Server> Server { get; set; }
        public virtual DbSet<Set> Set { get; set; }
        public virtual DbSet<ShipmentFiles> ShipmentFiles { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<TelemetrySummaryDaywise> TelemetrySummaryDaywise { get; set; }
        public virtual DbSet<TelemetrySummaryHourwise> TelemetrySummaryHourwise { get; set; }
        public virtual DbSet<Trip> Trip { get; set; }
        public virtual DbSet<TripStops> TripStops { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserDasboardWidget> UserDasboardWidget { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connString = component.helper.SolutionConfiguration.Configuration.ConnectionString;
                optionsBuilder.UseSqlServer(connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminRule>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__AdminRul__497F6CB43AD20F8C");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.AttributeGuid)
                    .HasColumnName("attributeGuid")
                    .HasColumnType("xml");

                entity.Property(e => e.CommandText)
                    .HasColumnName("commandText")
                    .HasMaxLength(500);

                entity.Property(e => e.CommandValue)
                    .HasColumnName("commandValue")
                    .HasMaxLength(100);

                entity.Property(e => e.ConditionText)
                    .IsRequired()
                    .HasColumnName("conditionText")
                    .HasMaxLength(1000);

                entity.Property(e => e.ConditionValue)
                    .IsRequired()
                    .HasColumnName("conditionValue")
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.NotificationType).HasColumnName("notificationType");

                entity.Property(e => e.RuleType)
                    .HasColumnName("ruleType")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SeverityLevelGuid).HasColumnName("severityLevelGuid");

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(e => e.Guid);

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.ContactNo)
                    .HasColumnName("contactNo")
                    .HasMaxLength(25);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AttributeValue>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Attribut__497F6CB48FBD0F85");

                entity.ToTable("AttributeValue", "IOTConnect");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.AggregateType).HasColumnName("aggregateType");

                entity.Property(e => e.AttributeValue1)
                    .HasColumnName("attributeValue")
                    .HasMaxLength(1000);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.DeviceUpdatedDate)
                    .HasColumnName("deviceUpdatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.GatewayUpdatedDate)
                    .HasColumnName("gatewayUpdatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocalName)
                    .HasColumnName("localName")
                    .HasMaxLength(100);

                entity.Property(e => e.SdkUpdatedDate)
                    .HasColumnName("sdkUpdatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(200);

                entity.Property(e => e.UniqueId)
                    .HasColumnName("uniqueId")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Company__497F6CB48E9359A1");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(500);

                entity.Property(e => e.AdminUserGuid).HasColumnName("adminUserGuid");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50);

                entity.Property(e => e.ContactNo)
                    .HasColumnName("contactNo")
                    .HasMaxLength(25);

                entity.Property(e => e.CountryGuid).HasColumnName("countryGuid");

                entity.Property(e => e.CpId)
                    .IsRequired()
                    .HasColumnName("cpId")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.ParentGuid).HasColumnName("parentGuid");

                entity.Property(e => e.PostalCode)
                    .HasColumnName("postalCode")
                    .HasMaxLength(30);

                entity.Property(e => e.StateGuid).HasColumnName("stateGuid");

                entity.Property(e => e.TimezoneGuid).HasColumnName("timezoneGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CompanyConfig>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__CompanyC__497F6CB4D68062C5");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.ConfigurationGuid).HasColumnName("configurationGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Configur__497F6CB49F167D72");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.ConfigKey)
                    .IsRequired()
                    .HasColumnName("configKey")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .HasName("CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<DebugInfo>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data");

                entity.Property(e => e.Dt)
                    .HasColumnName("dt")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Device__497F6CB42A362A73");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.FleetGuid).HasColumnName("fleetGuid");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsProvisioned).HasColumnName("isProvisioned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(1000);

                entity.Property(e => e.ParentDeviceGuid).HasColumnName("parentDeviceGuid");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasMaxLength(1000);

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(50);

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");

                entity.Property(e => e.TypeGuid).HasColumnName("typeGuid");

                entity.Property(e => e.UniqueId)
                    .IsRequired()
                    .HasColumnName("uniqueId")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<DeviceFiles>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__DeviceFiles__GUID");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.DeviceGuid).HasColumnName("deviceGuid");

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasColumnName("filePath")
                    .HasMaxLength(500);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<DeviceMaintenance>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Device__497F6CB4D51981B3");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.DeviceGuid).HasColumnName("deviceGuid");

                entity.Property(e => e.EndDateTime)
                    .HasColumnName("endDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
                entity.Property(e => e.IsCompleted).HasColumnName("isCompleted");
                entity.Property(e => e.StartDateTime)
                    .HasColumnName("startDateTime")
                    .HasColumnType("datetime");
                entity.Property(e => e.CompletedDate)
                   .HasColumnName("completedDate")
                   .HasColumnType("datetime");
            });

            modelBuilder.Entity<DeviceType>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK_Device_Guid");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Make)
                    .IsRequired()
                    .HasColumnName("make")
                    .HasMaxLength(200);

                entity.Property(e => e.Manufacturer)
                    .IsRequired()
                    .HasColumnName("manufacturer")
                    .HasMaxLength(200);

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnName("model")
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Driver__497F6CB4FD41A318");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(500);

                entity.Property(e => e.AggressiveAcceleration).HasColumnName("aggressiveAcceleration");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.ContactNo)
                    .HasColumnName("contactNo")
                    .HasMaxLength(25);

                entity.Property(e => e.CountryGuid).HasColumnName("countryGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.DriverId)
                    .HasColumnName("driverId")
                    .HasMaxLength(150);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(50);

                entity.Property(e => e.FleetGuid).HasColumnName("fleetGuid");

                entity.Property(e => e.HaltTime).HasColumnName("haltTime");

                entity.Property(e => e.HarshBraking).HasColumnName("harshBraking");

                entity.Property(e => e.IdleTime).HasColumnName("idleTime");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(50);

                entity.Property(e => e.LicenceImage)
                    .HasColumnName("licenceImage")
                    .HasMaxLength(250);

                entity.Property(e => e.LicenceNo)
                    .IsRequired()
                    .HasColumnName("licenceNo")
                    .HasMaxLength(25);

                entity.Property(e => e.OverSpeed).HasColumnName("overSpeed");

                entity.Property(e => e.StateGuid).HasColumnName("stateGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Zipcode)
                    .HasColumnName("zipcode")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__GreenHou__497F6CB475C7652E");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(500);

                entity.Property(e => e.Address2)
                    .HasColumnName("address2")
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CountryGuid).HasColumnName("countryGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);

                entity.Property(e => e.ParentEntityGuid).HasColumnName("parentEntityGuid");

                entity.Property(e => e.StateGuid).HasColumnName("stateGuid");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Zipcode)
                    .HasColumnName("zipcode")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Fleet>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Fleet__497F6CB475C7652E");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.FleetId)
                    .IsRequired()
                    .HasColumnName("fleetId")
                    .HasMaxLength(100);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasMaxLength(50);

                entity.Property(e => e.LoadingCapacity)
                    .HasColumnName("loadingCapacity")
                    .HasMaxLength(100);

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasMaxLength(50);

                entity.Property(e => e.MaterialTypeGuid).HasColumnName("materialTypeGuid");

                entity.Property(e => e.Radius)
                    .HasColumnName("radius")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RegistrationNo)
                    .HasColumnName("registrationNo")
                    .HasMaxLength(100);

                entity.Property(e => e.SpeedLimit)
                    .HasColumnName("speedLimit")
                    .HasMaxLength(250);

                entity.Property(e => e.TotalMiles)
                    .HasColumnName("totalMiles")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TypeGuid).HasColumnName("typeGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<FleetHaltStatus>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__FleetHaltStatus__497F6CB475C7652E");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.DriverGuid).HasColumnName("driverGuid");

                entity.Property(e => e.FleetGuid).HasColumnName("fleetGuid");

                entity.Property(e => e.HaltEndDateTime)
                    .HasColumnName("haltEndDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.HaltStartDateTime)
                    .HasColumnName("haltStartDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedHaltDateTime)
                    .HasColumnName("lastUpdatedHaltDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.TripGuid)
                    .HasColumnName("tripGuid")
                    .HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<FleetIdleStatus>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__FleetStatus__497F6CB475C7652E");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.DriverGuid).HasColumnName("driverGuid");

                entity.Property(e => e.FleetGuid).HasColumnName("fleetGuid");

                entity.Property(e => e.IdleEndDateTime)
                    .HasColumnName("idleEndDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdleStartDateTime)
                    .HasColumnName("idleStartDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedIdleDateTime)
                    .HasColumnName("lastUpdatedIdleDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.TripGuid)
                    .HasColumnName("tripGuid")
                    .HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<FleetMaterialType>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK_FleetMaterialType_Guid");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<FleetType>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK_FleetType_Guid");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<HardwareKit>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Hardware__497F6CB4048EB41D");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsProvisioned)
                    .IsRequired()
                    .HasColumnName("isProvisioned")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.KitCode)
                    .IsRequired()
                    .HasColumnName("kitCode")
                    .HasMaxLength(50);

                entity.Property(e => e.KitTypeGuid).HasColumnName("kitTypeGuid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasMaxLength(1000);

                entity.Property(e => e.TagGuid).HasColumnName("tagGuid");

                entity.Property(e => e.UniqueId)
                    .IsRequired()
                    .HasColumnName("uniqueId")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<IotconnectAlert>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__IOTConne__497F6CB4E001A8AA");

                entity.ToTable("IOTConnectAlert");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Audience)
                    .HasColumnName("audience")
                    .HasMaxLength(2000);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.Condition)
                    .HasColumnName("condition")
                    .HasMaxLength(2000);

                entity.Property(e => e.Data)
                    .HasColumnName("data")
                    .HasMaxLength(4000);

                entity.Property(e => e.DeviceGuid).HasColumnName("deviceGuid");

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.EventDate)
                    .HasColumnName("eventDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.EventId)
                    .HasColumnName("eventId")
                    .HasMaxLength(50);

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasMaxLength(500);

                entity.Property(e => e.RefGuid).HasColumnName("refGuid");

                entity.Property(e => e.RuleName)
                    .HasColumnName("ruleName")
                    .HasMaxLength(200);

                entity.Property(e => e.Severity)
                    .HasColumnName("severity")
                    .HasMaxLength(200);

                entity.Property(e => e.UniqueId)
                    .HasColumnName("uniqueId")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameter)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<KitType>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__KitType__497F6CB4F9806AB2");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasMaxLength(50);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<KitTypeAttribute>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__KitTypeA__497F6CB4FAAB409E");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(100);

                entity.Property(e => e.LocalName)
                    .IsRequired()
                    .HasColumnName("localName")
                    .HasMaxLength(100);

                entity.Property(e => e.ParentTemplateAttributeGuid).HasColumnName("parentTemplateAttributeGuid");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(50);

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");
            });

            modelBuilder.Entity<KitTypeCommand>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Command)
                    .HasColumnName("command")
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.Guid).HasColumnName("guid");

                entity.Property(e => e.IsOtacommand).HasColumnName("isOTACommand");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.RequiredAck).HasColumnName("requiredAck");

                entity.Property(e => e.RequiredParam).HasColumnName("requiredParam");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MasterWidget>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__MasterWi__A2B5777C65B643A6");

                entity.Property(e => e.Guid).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Widgets).HasColumnType("text");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Module__497F6CB45A20CC57");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.ApplyTo).HasColumnName("applyTo");

                entity.Property(e => e.CategoryName)
                    .HasColumnName("categoryName")
                    .HasMaxLength(200);

                entity.Property(e => e.IsAdminModule).HasColumnName("isAdminModule");

                entity.Property(e => e.ModuleSequence).HasColumnName("moduleSequence");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Permission).HasColumnName("permission");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Role__497F6CB433C166E2");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsAdminRole).HasColumnName("isAdminRole");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<RoleModulePermission>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__RoleModu__497F6CB4855A9952");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ModuleGuid).HasColumnName("moduleGuid");

                entity.Property(e => e.Permission).HasColumnName("permission");

                entity.Property(e => e.RoleGuid).HasColumnName("roleGuid");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShipmentFiles>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__ShipmentFiles__GUID");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasColumnName("filePath")
                    .HasMaxLength(500);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.TripGuid).HasColumnName("tripGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<TelemetrySummaryDaywise>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Telemetr__497F6CB47EFCA817");

                entity.ToTable("TelemetrySummary_Daywise");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Attribute)
                    .HasColumnName("attribute")
                    .HasMaxLength(1000);

                entity.Property(e => e.Avg)
                    .HasColumnName("avg")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.DeviceGuid).HasColumnName("deviceGuid");

                entity.Property(e => e.Latest)
                    .HasColumnName("latest")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Max)
                    .HasColumnName("max")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Min)
                    .HasColumnName("min")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Sum)
                    .HasColumnName("sum")
                    .HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<TelemetrySummaryHourwise>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Telemetr__497F6CB458372B0E");

                entity.ToTable("TelemetrySummary_Hourwise");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Attribute)
                    .HasColumnName("attribute")
                    .HasMaxLength(1000);

                entity.Property(e => e.Avg)
                    .HasColumnName("avg")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DeviceGuid).HasColumnName("deviceGuid");

                entity.Property(e => e.Latest)
                    .HasColumnName("latest")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Max)
                    .HasColumnName("max")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Min)
                    .HasColumnName("min")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Sum)
                    .HasColumnName("sum")
                    .HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Trip__497F6CB475C7652E");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActualStartDateTime)
                    .HasColumnName("actualStartDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.AggressiveAcceleration).HasColumnName("aggressiveAcceleration");

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CompletedDate)
                    .HasColumnName("completedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.CoveredMiles).HasColumnName("coveredMiles");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.DestinationLatitude)
                    .HasColumnName("destinationLatitude")
                    .HasMaxLength(50);

                entity.Property(e => e.DestinationLocation)
                    .HasColumnName("destinationLocation")
                    .HasMaxLength(250);

                entity.Property(e => e.DestinationLongitude)
                    .HasColumnName("destinationLongitude")
                    .HasMaxLength(50);

                entity.Property(e => e.EtaEndDateTime)
                    .HasColumnName("etaEndDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FleetGuid).HasColumnName("fleetGuid");

                entity.Property(e => e.HarshBraking).HasColumnName("harshBraking");

                entity.Property(e => e.IdleTime).HasColumnName("idleTime");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsCompleted).HasColumnName("isCompleted");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsStarted).HasColumnName("isStarted");

                entity.Property(e => e.MaterialTypeGuid).HasColumnName("materialTypeGuid");

                entity.Property(e => e.OverSpeed).HasColumnName("overSpeed");

                entity.Property(e => e.SourceLatitude)
                    .HasColumnName("sourceLatitude")
                    .HasMaxLength(50);

                entity.Property(e => e.SourceLocation)
                    .IsRequired()
                    .HasColumnName("sourceLocation")
                    .HasMaxLength(250);

                entity.Property(e => e.SourceLongitude)
                    .HasColumnName("sourceLongitude")
                    .HasMaxLength(50);

                entity.Property(e => e.StartDateTime)
                    .HasColumnName("startDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.TotalMiles)
                    .HasColumnName("totalMiles")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TripId)
                    .HasColumnName("tripId")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Weight)
                    .IsRequired()
                    .HasColumnName("weight")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TripStops>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__TripStops__GUID");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateTime)
                    .HasColumnName("endDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Latitude).HasMaxLength(50);

                entity.Property(e => e.Longitude).HasMaxLength(50);

                entity.Property(e => e.StopName)
                    .HasColumnName("stopName")
                    .HasMaxLength(200);

                entity.Property(e => e.TripGuid).HasColumnName("tripGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__User__497F6CB4FD41A318");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.ContactNo)
                    .HasColumnName("contactNo")
                    .HasMaxLength(25);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(50);

                entity.Property(e => e.ImageName)
                    .HasColumnName("imageName")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(50);

                entity.Property(e => e.RoleGuid).HasColumnName("roleGuid");

                entity.Property(e => e.TimeZoneGuid).HasColumnName("timeZoneGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
                entity.Property(e => e.SubscriptionEndDate)
                   .HasColumnName("subscriptionEndDate")
                   .HasColumnType("datetime");
            });

            modelBuilder.Entity<UserDasboardWidget>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__UserDasb__A2B5777C6DB81BD3");

                entity.Property(e => e.Guid).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DashboardName).HasMaxLength(100);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Widgets)
                    .IsRequired()
                    .HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
