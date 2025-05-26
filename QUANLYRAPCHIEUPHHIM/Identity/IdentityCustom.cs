using Microsoft.AspNetCore.Identity;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class AppRole : IdentityRole<int> { }
public class AppUserClaim : IdentityUserClaim<int> { }
public class AppUserRole : IdentityUserRole<int> { }
public class AppUserLogin : IdentityUserLogin<int> { }
public class AppRoleClaim : IdentityRoleClaim<int> { }
public class AppUserToken : IdentityUserToken<int> { }