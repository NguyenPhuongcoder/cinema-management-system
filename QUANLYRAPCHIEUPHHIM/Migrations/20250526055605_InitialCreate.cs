using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANLYRAPCHIEUPHHIM.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingStatus",
                columns: table => new
                {
                    booking_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingS__B02F4E9EBBDED263", x => x.booking_status_id);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    discount_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    discount_value = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    coupon_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    usage_limit = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Discount__BDBE9EF90B197F50", x => x.discount_id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountType",
                columns: table => new
                {
                    discount_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Discount__61DC13AA4708DE68", x => x.discount_type_id);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    genre_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    genre_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Genre__18428D423DA4D0E0", x => x.genre_id);
                });

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    release_date = table.Column<DateOnly>(type: "date", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    age_limit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    base_price = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    poster_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    panel_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    trailer_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    rating = table.Column<double>(type: "float", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    subtitles = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Movie__83CDF749A1C50F9E", x => x.movie_id);
                });

            migrationBuilder.CreateTable(
                name: "MoviePerson",
                columns: table => new
                {
                    person_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    biography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MoviePer__543848DF0D2E81D4", x => x.person_id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    payment_method_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    method_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentM__8A3EA9EB254402DF", x => x.payment_method_id);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    province_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    province_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Province__08DCB60FC3C10165", x => x.province_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__760965CC1234474A", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "RoleType",
                columns: table => new
                {
                    role_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RoleType__58914784B251A959", x => x.role_type_id);
                });

            migrationBuilder.CreateTable(
                name: "RoomFormat",
                columns: table => new
                {
                    format_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    format_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    additional_charge = table.Column<decimal>(type: "decimal(8,2)", nullable: true, defaultValue: 0.00m),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RoomForm__26B11DF134B7BF60", x => x.format_id);
                });

            migrationBuilder.CreateTable(
                name: "SeatType",
                columns: table => new
                {
                    seat_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    additional_charge = table.Column<decimal>(type: "decimal(8,2)", nullable: true, defaultValue: 0.00m),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SeatType__5C2EB19760C83778", x => x.seat_type_id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    registration_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__B9BE370F07D83566", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountDiscountType",
                columns: table => new
                {
                    discount_discount_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_id = table.Column<int>(type: "int", nullable: false),
                    discount_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Discount__A4380354AA9F4286", x => x.discount_discount_type_id);
                    table.ForeignKey(
                        name: "FK__DiscountD__disco__1F98B2C1",
                        column: x => x.discount_id,
                        principalTable: "Discount",
                        principalColumn: "discount_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__DiscountD__disco__208CD6FA",
                        column: x => x.discount_type_id,
                        principalTable: "DiscountType",
                        principalColumn: "discount_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenre",
                columns: table => new
                {
                    movie_genre_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    genre_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MovieGen__FE9D0DC61BFDC743", x => x.movie_genre_id);
                    table.ForeignKey(
                        name: "FK__MovieGenr__genre__797309D9",
                        column: x => x.genre_id,
                        principalTable: "Genre",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__MovieGenr__movie__787EE5A0",
                        column: x => x.movie_id,
                        principalTable: "Movie",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieCast",
                columns: table => new
                {
                    movie_cast_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    character_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MovieCas__00C3DA8502C1C510", x => x.movie_cast_id);
                    table.ForeignKey(
                        name: "FK__MovieCast__movie__02FC7413",
                        column: x => x.movie_id,
                        principalTable: "Movie",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__MovieCast__perso__03F0984C",
                        column: x => x.person_id,
                        principalTable: "MoviePerson",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    city_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    city_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    province_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__City__031491A87F3C4E2E", x => x.city_id);
                    table.ForeignKey(
                        name: "FK__City__province_i__3A81B327",
                        column: x => x.province_id,
                        principalTable: "Province",
                        principalColumn: "province_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieFormat",
                columns: table => new
                {
                    movie_format_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    format_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MovieFor__E435C60BBD0DD678", x => x.movie_format_id);
                    table.ForeignKey(
                        name: "FK__MovieForm__forma__7D439ABD",
                        column: x => x.format_id,
                        principalTable: "RoomFormat",
                        principalColumn: "format_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__MovieForm__movie__7C4F7684",
                        column: x => x.movie_id,
                        principalTable: "Movie",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    booking_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    total_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    discount_id = table.Column<int>(type: "int", nullable: true),
                    payment_due_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__5DE3A5B1C0C00224", x => x.booking_id);
                    table.ForeignKey(
                        name: "FK__Booking__discoun__2EDAF651",
                        column: x => x.discount_id,
                        principalTable: "Discount",
                        principalColumn: "discount_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__Booking__user_id__2DE6D218",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    user_role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__B8D9ABA27171D89B", x => x.user_role_id);
                    table.ForeignKey(
                        name: "FK__UserRole__role_i__5070F446",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__UserRole__user_i__4F7CD00D",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieCastRoleType",
                columns: table => new
                {
                    movie_cast_role_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_cast_id = table.Column<int>(type: "int", nullable: false),
                    role_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MovieCas__C7097E3A10D56D50", x => x.movie_cast_role_type_id);
                    table.ForeignKey(
                        name: "FK__MovieCast__movie__07C12930",
                        column: x => x.movie_cast_id,
                        principalTable: "MovieCast",
                        principalColumn: "movie_cast_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__MovieCast__role___08B54D69",
                        column: x => x.role_type_id,
                        principalTable: "RoleType",
                        principalColumn: "role_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address_detail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    city_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Address__CAA247C899E5234E", x => x.address_id);
                    table.ForeignKey(
                        name: "FK__Address__city_id__3D5E1FD2",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "city_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingBookingStatus",
                columns: table => new
                {
                    booking_booking_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    booking_status_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingB__3880CD97013DEAE3", x => x.booking_booking_status_id);
                    table.ForeignKey(
                        name: "FK__BookingBo__booki__32AB8735",
                        column: x => x.booking_id,
                        principalTable: "Booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__BookingBo__booki__339FAB6E",
                        column: x => x.booking_status_id,
                        principalTable: "BookingStatus",
                        principalColumn: "booking_status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    payment_method_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    transaction_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    payment_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__ED1FC9EA9281E88E", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK__Payment__booking__367C1819",
                        column: x => x.booking_id,
                        principalTable: "Booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Payment__payment__37703C52",
                        column: x => x.payment_method_id,
                        principalTable: "PaymentMethod",
                        principalColumn: "payment_method_id");
                });

            migrationBuilder.CreateTable(
                name: "Cinema",
                columns: table => new
                {
                    cinema_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cinema_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cinema__5662877880FA2573", x => x.cinema_id);
                    table.ForeignKey(
                        name: "FK__Cinema__address___4222D4EF",
                        column: x => x.address_id,
                        principalTable: "Address",
                        principalColumn: "address_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cinema_id = table.Column<int>(type: "int", nullable: false),
                    format_id = table.Column<int>(type: "int", nullable: false),
                    room_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Room__19675A8AE015982B", x => x.room_id);
                    table.ForeignKey(
                        name: "FK__Room__cinema_id__59FA5E80",
                        column: x => x.cinema_id,
                        principalTable: "Cinema",
                        principalColumn: "cinema_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Room__format_id__5AEE82B9",
                        column: x => x.format_id,
                        principalTable: "RoomFormat",
                        principalColumn: "format_id");
                });

            migrationBuilder.CreateTable(
                name: "Seat",
                columns: table => new
                {
                    seat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    seat_type_id = table.Column<int>(type: "int", nullable: false),
                    row_letter = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    seat_number = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seat__906DED9C93B811CF", x => x.seat_id);
                    table.ForeignKey(
                        name: "FK__Seat__room_id__66603565",
                        column: x => x.room_id,
                        principalTable: "Room",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Seat__seat_type___6754599E",
                        column: x => x.seat_type_id,
                        principalTable: "SeatType",
                        principalColumn: "seat_type_id");
                });

            migrationBuilder.CreateTable(
                name: "Showtime",
                columns: table => new
                {
                    showtime_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    price_modifier = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0.00m),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Showtime__A406B518C4D6AA7E", x => x.showtime_id);
                    table.ForeignKey(
                        name: "FK__Showtime__movie___0B91BA14",
                        column: x => x.movie_id,
                        principalTable: "Movie",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Showtime__room_i__0C85DE4D",
                        column: x => x.room_id,
                        principalTable: "Room",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    ticket_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    showtime_id = table.Column<int>(type: "int", nullable: false),
                    seat_id = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ticket_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ticket_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    scan_datetime = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ticket__D596F96B7CBFCD39", x => x.ticket_id);
                    table.ForeignKey(
                        name: "FK__Ticket__TicketSt__3E1D39E1",
                        column: x => x.booking_id,
                        principalTable: "Booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Ticket__seat_id__40058253",
                        column: x => x.seat_id,
                        principalTable: "Seat",
                        principalColumn: "seat_id");
                    table.ForeignKey(
                        name: "FK__Ticket__showtime__3F115E1A",
                        column: x => x.showtime_id,
                        principalTable: "Showtime",
                        principalColumn: "showtime_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_city_id",
                table: "Address",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "idx_booking_date",
                table: "Booking",
                column: "booking_date");

            migrationBuilder.CreateIndex(
                name: "idx_booking_user",
                table: "Booking",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_discount_id",
                table: "Booking",
                column: "discount_id");

            migrationBuilder.CreateIndex(
                name: "IX_BookingBookingStatus_booking_status_id",
                table: "BookingBookingStatus",
                column: "booking_status_id");

            migrationBuilder.CreateIndex(
                name: "UC_BookingBookingStatus",
                table: "BookingBookingStatus",
                columns: new[] { "booking_id", "booking_status_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__BookingS__0A422CD5396FF74F",
                table: "BookingStatus",
                column: "booking_status_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cinema_address_id",
                table: "Cinema",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_City_province_id",
                table: "City",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Discount__ADE5CBB776BB8E6A",
                table: "Discount",
                column: "coupon_code",
                unique: true,
                filter: "[coupon_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountDiscountType_discount_type_id",
                table: "DiscountDiscountType",
                column: "discount_type_id");

            migrationBuilder.CreateIndex(
                name: "UC_DiscountDiscountType",
                table: "DiscountDiscountType",
                columns: new[] { "discount_id", "discount_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Discount__805CD7FE0A6BE97C",
                table: "DiscountType",
                column: "discount_type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Genre__1E98D15134FDBC71",
                table: "Genre",
                column: "genre_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_movie_title",
                table: "Movie",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCast_movie_id",
                table: "MovieCast",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCast_person_id",
                table: "MovieCast",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCastRoleType_role_type_id",
                table: "MovieCastRoleType",
                column: "role_type_id");

            migrationBuilder.CreateIndex(
                name: "UC_MovieCastRoleType",
                table: "MovieCastRoleType",
                columns: new[] { "movie_cast_id", "role_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieFormat_format_id",
                table: "MovieFormat",
                column: "format_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieFormat_movie_id",
                table: "MovieFormat",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenre_genre_id",
                table: "MovieGenre",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenre_movie_id",
                table: "MovieGenre",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_booking_id",
                table: "Payment",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_payment_method_id",
                table: "Payment",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "UQ__PaymentM__2DA2FAEE73397403",
                table: "PaymentMethod",
                column: "method_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Province__D6FBADF9C7CDB56F",
                table: "Province",
                column: "province_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Role__783254B1D380F08E",
                table: "Role",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__RoleType__DDC330EE3629E291",
                table: "RoleType",
                column: "role_type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Room_format_id",
                table: "Room",
                column: "format_id");

            migrationBuilder.CreateIndex(
                name: "UC_Room",
                table: "Room",
                columns: new[] { "cinema_id", "room_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__RoomForm__8D68276D7852F48E",
                table: "RoomFormat",
                column: "format_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seat_seat_type_id",
                table: "Seat",
                column: "seat_type_id");

            migrationBuilder.CreateIndex(
                name: "UC_Seat",
                table: "Seat",
                columns: new[] { "room_id", "row_letter", "seat_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SeatType__543C4FD920CE7E76",
                table: "SeatType",
                column: "type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_showtime_start",
                table: "Showtime",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_Showtime_movie_id",
                table: "Showtime",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_Showtime_room_id",
                table: "Showtime",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "idx_ticket_code",
                table: "Ticket",
                column: "ticket_code");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_booking_id",
                table: "Ticket",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_seat_id",
                table: "Ticket",
                column: "seat_id");

            migrationBuilder.CreateIndex(
                name: "UC_Ticket",
                table: "Ticket",
                columns: new[] { "showtime_id", "seat_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Ticket__628DB75F81D384B2",
                table: "Ticket",
                column: "ticket_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "idx_user_email",
                table: "User",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "UQ__User__AB6E61642EF7F8C8",
                table: "User",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__User__F3DBC5729DB2EDAD",
                table: "User",
                column: "username",
                unique: true,
                filter: "[username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_role_id",
                table: "UserRole",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UC_UserRole",
                table: "UserRole",
                columns: new[] { "user_id", "role_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookingBookingStatus");

            migrationBuilder.DropTable(
                name: "DiscountDiscountType");

            migrationBuilder.DropTable(
                name: "MovieCastRoleType");

            migrationBuilder.DropTable(
                name: "MovieFormat");

            migrationBuilder.DropTable(
                name: "MovieGenre");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BookingStatus");

            migrationBuilder.DropTable(
                name: "DiscountType");

            migrationBuilder.DropTable(
                name: "MovieCast");

            migrationBuilder.DropTable(
                name: "RoleType");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "Showtime");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "MoviePerson");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "SeatType");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Cinema");

            migrationBuilder.DropTable(
                name: "RoomFormat");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Province");
        }
    }
}
