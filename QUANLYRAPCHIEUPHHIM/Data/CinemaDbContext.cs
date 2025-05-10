using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.Data;

public partial class CinemaDbContext : DbContext
{
    public CinemaDbContext()
    {
    }

    public CinemaDbContext(DbContextOptions<CinemaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingBookingStatus> BookingBookingStatuses { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountDiscountType> DiscountDiscountTypes { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieCast> MovieCasts { get; set; }

    public virtual DbSet<MovieCastRoleType> MovieCastRoleTypes { get; set; }

    public virtual DbSet<MovieFormat> MovieFormats { get; set; }

    public virtual DbSet<MovieGenre> MovieGenres { get; set; }

    public virtual DbSet<MoviePerson> MoviePeople { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentPaymentStatus> PaymentPaymentStatuses { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleType> RoleTypes { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomFormat> RoomFormats { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatType> SeatTypes { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<TicketTicketStatus> TicketTicketStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Address__CAA247C879A335FE");

            entity.ToTable("Address");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.AddressDetail)
                .HasMaxLength(200)
                .HasColumnName("address_detail");
            entity.Property(e => e.CityId).HasColumnName("city_id");

            entity.HasOne(d => d.City).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK__Address__city_id__3D5E1FD2");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__5DE3A5B1A5E3D2EC");

            entity.ToTable("Booking");

            entity.HasIndex(e => e.BookingDate, "idx_booking_date");

            entity.HasIndex(e => e.UserId, "idx_booking_user");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.PaymentDueDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_due_date");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Discount).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Booking__discoun__31B762FC");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__user_id__30C33EC3");
        });

        modelBuilder.Entity<BookingBookingStatus>(entity =>
        {
            entity.HasKey(e => e.BookingBookingStatusId).HasName("PK__BookingB__3880CD97EF93F379");

            entity.ToTable("BookingBookingStatus");

            entity.HasIndex(e => new { e.BookingId, e.BookingStatusId }, "UC_BookingBookingStatus").IsUnique();

            entity.Property(e => e.BookingBookingStatusId).HasColumnName("booking_booking_status_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingStatusId).HasColumnName("booking_status_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingBookingStatuses)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingBo__booki__3587F3E0");

            entity.HasOne(d => d.BookingStatus).WithMany(p => p.BookingBookingStatuses)
                .HasForeignKey(d => d.BookingStatusId)
                .HasConstraintName("FK__BookingBo__booki__367C1819");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.BookingStatusId).HasName("PK__BookingS__B02F4E9ED900AD75");

            entity.ToTable("BookingStatus");

            entity.HasIndex(e => e.BookingStatusName, "UQ__BookingS__0A422CD5112DB458").IsUnique();

            entity.Property(e => e.BookingStatusId).HasColumnName("booking_status_id");
            entity.Property(e => e.BookingStatusName)
                .HasMaxLength(50)
                .HasColumnName("booking_status_name");
        });

        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.CinemaId).HasName("PK__Cinema__566287788A357AD9");

            entity.ToTable("Cinema");

            entity.Property(e => e.CinemaId).HasColumnName("cinema_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.CinemaName)
                .HasMaxLength(100)
                .HasColumnName("cinema_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Address).WithMany(p => p.Cinemas)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__Cinema__address___4222D4EF");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__031491A8222326E2");

            entity.ToTable("City");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .HasColumnName("city_name");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");

            entity.HasOne(d => d.Province).WithMany(p => p.Cities)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("FK__City__province_i__3A81B327");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__BDBE9EF950F315E2");

            entity.ToTable("Discount");

            entity.HasIndex(e => e.CouponCode, "UQ__Discount__ADE5CBB73F5E2FCC").IsUnique();

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.CouponCode)
                .HasMaxLength(20)
                .HasColumnName("coupon_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountName)
                .HasMaxLength(100)
                .HasColumnName("discount_name");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsageLimit)
                .HasDefaultValue(0)
                .HasColumnName("usage_limit");
        });

        modelBuilder.Entity<DiscountDiscountType>(entity =>
        {
            entity.HasKey(e => e.DiscountDiscountTypeId).HasName("PK__Discount__A43803541B6F2AA4");

            entity.ToTable("DiscountDiscountType");

            entity.HasIndex(e => new { e.DiscountId, e.DiscountTypeId }, "UC_DiscountDiscountType").IsUnique();

            entity.Property(e => e.DiscountDiscountTypeId).HasColumnName("discount_discount_type_id");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.DiscountTypeId).HasColumnName("discount_type_id");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountDiscountTypes)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__DiscountD__disco__1F98B2C1");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.DiscountDiscountTypes)
                .HasForeignKey(d => d.DiscountTypeId)
                .HasConstraintName("FK__DiscountD__disco__208CD6FA");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.HasKey(e => e.DiscountTypeId).HasName("PK__Discount__61DC13AAB75906A3");

            entity.ToTable("DiscountType");

            entity.HasIndex(e => e.DiscountTypeName, "UQ__Discount__805CD7FE4AF03D10").IsUnique();

            entity.Property(e => e.DiscountTypeId).HasColumnName("discount_type_id");
            entity.Property(e => e.DiscountTypeName)
                .HasMaxLength(50)
                .HasColumnName("discount_type_name");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genre__18428D42CF8B59E0");

            entity.ToTable("Genre");

            entity.HasIndex(e => e.GenreName, "UQ__Genre__1E98D151E2E0A1A9").IsUnique();

            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GenreName)
                .HasMaxLength(50)
                .HasColumnName("genre_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movie__83CDF7497217B9EB");

            entity.ToTable("Movie");

            entity.HasIndex(e => e.Title, "idx_movie_title");

            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.AgeLimit)
                .HasMaxLength(10)
                .HasColumnName("age_limit");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("base_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Language)
                .HasMaxLength(50)
                .HasColumnName("language");
            entity.Property(e => e.PosterUrl)
                .HasMaxLength(255)
                .HasColumnName("poster_url");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Subtitles)
                .HasMaxLength(50)
                .HasColumnName("subtitles");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.TrailerUrl)
                .HasMaxLength(255)
                .HasColumnName("trailer_url");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<MovieCast>(entity =>
        {
            entity.HasKey(e => e.MovieCastId).HasName("PK__MovieCas__00C3DA852B81A27F");

            entity.ToTable("MovieCast");

            entity.Property(e => e.MovieCastId).HasColumnName("movie_cast_id");
            entity.Property(e => e.CharacterName)
                .HasMaxLength(100)
                .HasColumnName("character_name");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieCasts)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__MovieCast__movie__02FC7413");

            entity.HasOne(d => d.Person).WithMany(p => p.MovieCasts)
                .HasForeignKey(d => d.PersonId)
                .HasConstraintName("FK__MovieCast__perso__03F0984C");
        });

        modelBuilder.Entity<MovieCastRoleType>(entity =>
        {
            entity.HasKey(e => e.MovieCastRoleTypeId).HasName("PK__MovieCas__C7097E3A5CA1CD6F");

            entity.ToTable("MovieCastRoleType");

            entity.HasIndex(e => new { e.MovieCastId, e.RoleTypeId }, "UC_MovieCastRoleType").IsUnique();

            entity.Property(e => e.MovieCastRoleTypeId).HasColumnName("movie_cast_role_type_id");
            entity.Property(e => e.MovieCastId).HasColumnName("movie_cast_id");
            entity.Property(e => e.RoleTypeId).HasColumnName("role_type_id");

            entity.HasOne(d => d.MovieCast).WithMany(p => p.MovieCastRoleTypes)
                .HasForeignKey(d => d.MovieCastId)
                .HasConstraintName("FK__MovieCast__movie__07C12930");

            entity.HasOne(d => d.RoleType).WithMany(p => p.MovieCastRoleTypes)
                .HasForeignKey(d => d.RoleTypeId)
                .HasConstraintName("FK__MovieCast__role___08B54D69");
        });

        modelBuilder.Entity<MovieFormat>(entity =>
        {
            entity.HasKey(e => e.MovieFormatId).HasName("PK__MovieFor__E435C60B92BF4C7E");

            entity.ToTable("MovieFormat");

            entity.Property(e => e.MovieFormatId).HasColumnName("movie_format_id");
            entity.Property(e => e.FormatId).HasColumnName("format_id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");

            entity.HasOne(d => d.Format).WithMany(p => p.MovieFormats)
                .HasForeignKey(d => d.FormatId)
                .HasConstraintName("FK__MovieForm__forma__7D439ABD");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieFormats)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__MovieForm__movie__7C4F7684");
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.HasKey(e => e.MovieGenreId).HasName("PK__MovieGen__FE9D0DC6CBBEF83F");

            entity.ToTable("MovieGenre");

            entity.Property(e => e.MovieGenreId).HasColumnName("movie_genre_id");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");

            entity.HasOne(d => d.Genre).WithMany(p => p.MovieGenres)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK__MovieGenr__genre__797309D9");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieGenres)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__MovieGenr__movie__787EE5A0");
        });

        modelBuilder.Entity<MoviePerson>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__MoviePer__543848DF67054CED");

            entity.ToTable("MoviePerson");

            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.Biography).HasColumnName("biography");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Nationality)
                .HasMaxLength(50)
                .HasColumnName("nationality");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EA844B56AE");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payment__booking__395884C4");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__payment__3A4CA8FD");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__8A3EA9EB13936CA6");

            entity.ToTable("PaymentMethod");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__2DA2FAEEFBA5B1F2").IsUnique();

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MethodName)
                .HasMaxLength(50)
                .HasColumnName("method_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PaymentPaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentPaymentStatusId).HasName("PK__PaymentP__D2C8DF9D0305C39A");

            entity.ToTable("PaymentPaymentStatus");

            entity.HasIndex(e => new { e.PaymentId, e.PaymentStatusId }, "UC_PaymentPaymentStatus").IsUnique();

            entity.Property(e => e.PaymentPaymentStatusId).HasColumnName("payment_payment_status_id");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.PaymentStatusId).HasColumnName("payment_status_id");

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentPaymentStatuses)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__PaymentPa__payme__40058253");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.PaymentPaymentStatuses)
                .HasForeignKey(d => d.PaymentStatusId)
                .HasConstraintName("FK__PaymentPa__payme__40F9A68C");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentStatusId).HasName("PK__PaymentS__E6BF5015439E3022");

            entity.ToTable("PaymentStatus");

            entity.HasIndex(e => e.PaymentStatusName, "UQ__PaymentS__C28365FAEE211E22").IsUnique();

            entity.Property(e => e.PaymentStatusId).HasColumnName("payment_status_id");
            entity.Property(e => e.PaymentStatusName)
                .HasMaxLength(50)
                .HasColumnName("payment_status_name");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.ProvinceId).HasName("PK__Province__08DCB60F11E49023");

            entity.ToTable("Province");

            entity.HasIndex(e => e.ProvinceName, "UQ__Province__D6FBADF94C712E4F").IsUnique();

            entity.Property(e => e.ProvinceId).HasColumnName("province_id");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(100)
                .HasColumnName("province_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CC980DC6CE");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__783254B1FC110F3F").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<RoleType>(entity =>
        {
            entity.HasKey(e => e.RoleTypeId).HasName("PK__RoleType__58914784FB9C843B");

            entity.ToTable("RoleType");

            entity.HasIndex(e => e.RoleTypeName, "UQ__RoleType__DDC330EE0CFC3D5C").IsUnique();

            entity.Property(e => e.RoleTypeId).HasColumnName("role_type_id");
            entity.Property(e => e.RoleTypeName)
                .HasMaxLength(50)
                .HasColumnName("role_type_name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__19675A8ADF195FB5");

            entity.ToTable("Room");

            entity.HasIndex(e => new { e.CinemaId, e.RoomName }, "UC_Room").IsUnique();

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CinemaId).HasColumnName("cinema_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FormatId).HasColumnName("format_id");
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .HasColumnName("room_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Cinema).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.CinemaId)
                .HasConstraintName("FK__Room__cinema_id__59FA5E80");

            entity.HasOne(d => d.Format).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.FormatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Room__format_id__5AEE82B9");
        });

        modelBuilder.Entity<RoomFormat>(entity =>
        {
            entity.HasKey(e => e.FormatId).HasName("PK__RoomForm__26B11DF10B5F9E94");

            entity.ToTable("RoomFormat");

            entity.HasIndex(e => e.FormatName, "UQ__RoomForm__8D68276DA120D04C").IsUnique();

            entity.Property(e => e.FormatId).HasColumnName("format_id");
            entity.Property(e => e.AdditionalCharge)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("additional_charge");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.FormatName)
                .HasMaxLength(50)
                .HasColumnName("format_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__906DED9C8ED0D9F4");

            entity.ToTable("Seat");

            entity.HasIndex(e => new { e.RoomId, e.RowLetter, e.SeatNumber }, "UC_Seat").IsUnique();

            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RowLetter)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("row_letter");
            entity.Property(e => e.SeatNumber).HasColumnName("seat_number");
            entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Room).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__Seat__room_id__66603565");

            entity.HasOne(d => d.SeatType).WithMany(p => p.Seats)
                .HasForeignKey(d => d.SeatTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__seat_type___6754599E");
        });

        modelBuilder.Entity<SeatType>(entity =>
        {
            entity.HasKey(e => e.SeatTypeId).HasName("PK__SeatType__5C2EB197CF87EB95");

            entity.ToTable("SeatType");

            entity.HasIndex(e => e.TypeName, "UQ__SeatType__543C4FD9A6454C8C").IsUnique();

            entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");
            entity.Property(e => e.AdditionalCharge)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("additional_charge");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PK__Showtime__A406B518DC62C059");

            entity.ToTable("Showtime");

            entity.HasIndex(e => e.StartTime, "idx_showtime_start");

            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.PriceModifier)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("price_modifier");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__Showtime__movie___0B91BA14");

            entity.HasOne(d => d.Room).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__Showtime__room_i__0C85DE4D");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__D596F96BAE67648B");

            entity.ToTable("Ticket");

            entity.HasIndex(e => new { e.ShowtimeId, e.SeatId }, "UC_Ticket").IsUnique();

            entity.HasIndex(e => e.TicketCode, "UQ__Ticket__628DB75F11D491B6").IsUnique();

            entity.HasIndex(e => e.TicketCode, "idx_ticket_code");

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ScanDatetime)
                .HasColumnType("datetime")
                .HasColumnName("scan_datetime");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(20)
                .HasColumnName("ticket_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Ticket__booking___489AC854");

            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__seat_id__4A8310C6");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__showtime__498EEC8D");
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.TicketStatusId).HasName("PK__TicketSt__FB188BC17AC7325E");

            entity.ToTable("TicketStatus");

            entity.HasIndex(e => e.TicketStatusName, "UQ__TicketSt__9A757176A254553F").IsUnique();

            entity.Property(e => e.TicketStatusId).HasColumnName("ticket_status_id");
            entity.Property(e => e.TicketStatusName)
                .HasMaxLength(50)
                .HasColumnName("ticket_status_name");
        });

        modelBuilder.Entity<TicketTicketStatus>(entity =>
        {
            entity.HasKey(e => e.TicketTicketStatusId).HasName("PK__TicketTi__8341A40D5A1C1C38");

            entity.ToTable("TicketTicketStatus");

            entity.HasIndex(e => new { e.TicketId, e.TicketStatusId }, "UC_TicketTicketStatus").IsUnique();

            entity.Property(e => e.TicketTicketStatusId).HasColumnName("ticket_ticket_status_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.TicketStatusId).HasColumnName("ticket_status_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketTicketStatuses)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK__TicketTic__ticke__503BEA1C");

            entity.HasOne(d => d.TicketStatus).WithMany(p => p.TicketTicketStatuses)
                .HasForeignKey(d => d.TicketStatusId)
                .HasConstraintName("FK__TicketTic__ticke__51300E55");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370F7A2ED9EC");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E61643970E251").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__User__F3DBC5724E468EA5").IsUnique();

            entity.HasIndex(e => e.Email, "idx_user_email");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registration_date");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__B8D9ABA2F095841E");

            entity.ToTable("UserRole");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UC_UserRole").IsUnique();

            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__UserRole__role_i__5070F446");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserRole__user_i__4F7CD00D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
