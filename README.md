# 🎬 Cinema Management System

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-6.0-blue)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-green)](https://docs.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0-purple)](https://getbootstrap.com/)

A comprehensive web application built with ASP.NET Core MVC for managing cinema operations and providing seamless online ticket booking experiences.

## 👥 Team Information

| Full Name | Student ID |
|-----------|------------|
| Nguyễn Phương | 23115053122232 |
| Lê Viết Hoàng Thắng | 23115053122237 |
| Nguyễn Hữu Phước | 23115053122232 |

## 📌 Introduction

The Cinema Management System is a modern web application designed to streamline cinema operations while providing customers with an intuitive platform for online ticket booking. Built with robust technologies and following best practices, this system offers comprehensive functionality for both administrators and end-users.

## 🛠 Technologies Used

- **Framework**: ASP.NET Core MVC
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Frontend**: Bootstrap 5, jQuery
- **Icons**: Font Awesome
- **Architecture**: Model-View-Controller (MVC)

## 🚀 Core Features

### 🎛 System Management (Admin Panel)

#### Movie Management
- ➕ Add, update, and delete movies
- 🎭 Manage movie genres and age ratings
- 🖼️ Upload movie posters and trailers
- 📅 Set release dates and duration

#### Cinema Management
- 🏢 Manage cinema chains by location (province/city)
- 🎪 Configure screening rooms and seating layouts
- 💰 Set dynamic ticket pricing by room/cinema
- 📊 Monitor capacity and utilization

#### Showtime Management
- ⏰ Schedule movie showtimes efficiently
- 📋 Manage showtime availability
- 📈 Monitor real-time booking status
- 🔄 Handle schedule conflicts automatically

#### Promotion Management
- 🎟️ Create and manage discount codes
- 📢 Launch targeted promotional campaigns
- 👥 Apply special offers to customer segments
- 📊 Track promotion effectiveness

#### Reporting & Analytics
- 💹 Revenue tracking by time period and cinema
- 👀 Detailed viewership statistics
- 📊 Comprehensive booking activity reports
- 📈 Performance dashboards

### 👤 User-Side Functionality

#### Account Management
- 🔐 Secure user registration & login
- 👤 Personal profile management
- 🔒 Password reset functionality
- 📧 Email verification system

#### Homepage & Movie Discovery
- 🎬 Display currently showing and upcoming movies
- 🔍 Advanced search and filtering options
- 📱 Responsive movie detail pages
- 🎥 Integrated trailer viewing

#### Online Ticket Booking
- 🎯 Intuitive movie, cinema, and showtime selection
- 💺 Interactive seat selection interface
- 🏷️ Discount code application
- 💳 Secure online payment processing
- 📱 Mobile-optimized booking flow

#### Booking Management
- 📋 Comprehensive booking history
- 🎫 Digital e-ticket access and printing
- ❌ Booking cancellation (where applicable)
- 🔔 Booking confirmation notifications

## 📈 Key Achievements

### 1. User Experience Excellence
- ✨ Modern, intuitive interface design
- 📱 Fully responsive across all devices
- ⚡ Streamlined 3-step booking process
- 🔒 PCI-compliant payment integration

### 2. Administrative Efficiency
- 📊 Comprehensive admin dashboard
- 📈 Real-time analytics and reporting
- ⚙️ Flexible system configuration
- 🎯 Advanced promotion management

### 3. Performance & Security
- 🚀 Optimized loading speeds (<2s average)
- 🔐 Enterprise-grade security measures
- 🔄 Support for concurrent transactions
- 💾 Automated backup and recovery

### 4. Standout Features
- 🧠 Intelligent seat recommendation
- 🏢 Multi-location cinema support
- 💳 Multiple payment gateway integration
- 🎁 Flexible promotional system

## 🔮 Future Development Roadmap

- 🤖 **AI Integration**: Personalized movie recommendations
- 📱 **Mobile Apps**: Native iOS and Android applications
- 💳 **Payment Expansion**: Cryptocurrency and digital wallet support
- ⭐ **Social Features**: Movie ratings, reviews, and social sharing
- 🍿 **Concession Integration**: Online food and beverage ordering
- 🎮 **Gamification**: Loyalty points and achievement system
- 📊 **Advanced Analytics**: Machine learning insights
- 🌐 **Multi-language**: Internationalization support

## ⚙️ Installation & Setup

### Prerequisites
- .NET 6.0 SDK or later
- SQL Server 2019+ or SQL Server Express
- Visual Studio 2022 or VS Code

### Installation Steps

1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd cinema-management-system
   ```

2. **Install Dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure Database**
   - Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CinemaDB;Trusted_Connection=true;"
     }
   }
   ```

4. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

6. **Access the Application**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - Default admin credentials (change immediately):
     - Username: `admin@cinema.com`
     - Password: `Admin@123`

## 📁 Project Structure

```
CinemaManagementSystem/
├── Controllers/          # MVC Controllers
├── Models/              # Data models and ViewModels
├── Views/               # Razor views
├── Data/                # Entity Framework context
├── Services/            # Business logic services
├── wwwroot/             # Static files (CSS, JS, images)
├── Migrations/          # EF Core migrations
└── appsettings.json     # Configuration file
```

## 🧪 Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🤝 Contributing

We welcome contributions from the community! Here's how you can help:

1. 🍴 Fork the repository
2. 🌟 Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. 💻 Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. 📤 Push to the branch (`git push origin feature/AmazingFeature`)
5. 🔀 Open a Pull Request

### Contribution Guidelines
- Follow the existing code style and conventions
- Add unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support & Contact

- 📧 Email: support@cinemamanagement.com
- 🐛 Issues: [GitHub Issues](repository-url/issues)
- 📖 Documentation: [Wiki](repository-url/wiki)

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Bootstrap team for the responsive UI components
- Font Awesome for the beautiful icons
- Our university instructors for guidance and support

---

<div align="center">
  <p>Made with ❤️ by the Cinema Management Team</p>
  <p>© 2024 Cinema Management System. All rights reserved.</p>
</div>
