namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Player" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }

            try {
                SeedSports(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Sports in the Database.");
            }

            /*
            // Seed Teams
            try {
                SeedTeams(dbContext, userManager);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Teams in the Database.");
            }
            */

        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("tucalvitofavorito@gmail.com").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Isi", "Palazón", "tucalvitofavorito@gmail.com");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //player role
                    userManager.AddToRoleAsync(user, roles[1]).Wait();
                }
            }
            
            if (userManager.FindByNameAsync("charliekirk@gmail.com").Result == null) {
                ApplicationUser user = new ApplicationUser("2", "Charlie", "Kirk", "charliekirk@gmail.com");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //player role
                    userManager.AddToRoleAsync(user, roles[1]).Wait();
                }
            }

            if (userManager.FindByNameAsync("balicki@gmail.com").Result == null) {
                
                ApplicationUser user = new ApplicationUser("0", "Kryzstof", "Balicki", "balicki@gmail.com");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //admin role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();

                }
            }

        }

        public static void SeedSports(ApplicationDbContext dbcontext) 
        {
            if (dbcontext.Sports.FirstOrDefault(s => s.Name == "Football") == null) {
                // Usando tu constructor de Sport
                var sport = new Sport(0, "Football", "Team sport playing 11vs11", "Don't touch the ball with the hands, except the goalkeeper.") 
                { 
                    MinimumNumberofPlayers = 11,
                    NumberofReferees = 1
                };
                dbcontext.Sports.Add(sport);
            }

            if (dbcontext.Sports.FirstOrDefault(s => s.Name == "Padel") == null) {
                var sport = new Sport(1, "Padel", "Team sport playing with racquets.", "The ball can bounce off the walls.")
                {
                    MinimumNumberofPlayers = 2,
                    NumberofReferees = 1
                };
                dbcontext.Sports.Add(sport);
            }

            dbcontext.SaveChanges();
        }

        /*
        public static void SeedTeams(ApplicationDbContext dbcontext, UserManager<ApplicationUser> userManager) 
        {
            // Comprobamos si el equipo ya existe
            if (dbcontext.Teams.FirstOrDefault(t => t.Name == "Sporting Albacete") == null) {
                
                // Buscamos el deporte y el usuario que hemos creado en los métodos anteriores
                var sport = dbcontext.Sports.FirstOrDefault(s => s.Name == "Football");
                var captain = userManager.FindByNameAsync("charliekirk@gmail.com").Result;

                // Si ambos existen, creamos el equipo usando el constructor de tu compañero
                if (sport != null && captain != null) {
                    var team = new Team(
                        id: 0, 
                        name: "Sporting Albacete", 
                        description: "Amateur football team from Albacete.", 
                        maxMembers: 23, 
                        sportId: sport.Id, 
                        captainId: captain.Id
                    ) 
                    {
                        MinAge = 18,
                        MaxAge = 60
                    };
                    dbcontext.Teams.Add(team);
                }
            }

            dbcontext.SaveChanges();

            if (dbcontext.Teams.FirstOrDefault(t => t.Name == "Hoya-Gonzalo Clippers") == null) {
                
                // Buscamos el deporte y el usuario que hemos creado en los métodos anteriores
                var sport = dbcontext.Sports.FirstOrDefault(s => s.Name == "Football");
                var captain = userManager.FindByNameAsync("tucalvitofavorito@gmail.com").Result;

                // Si ambos existen, creamos el equipo usando el constructor de tu compañero
                if (sport != null && captain != null) {
                    var team = new Team(
                        id: 1, 
                        name: "Hoya-Gonzalo Clippers", 
                        description: "Professional football team from Hoya-Gonzalo.", 
                        maxMembers: 23, 
                        sportId: sport.Id, 
                        captainId: captain.Id
                    ) 
                    {
                        MinAge = 18,
                        MaxAge = 60
                    };
                    dbcontext.Teams.Add(team);
                }
            }

            dbcontext.SaveChanges();
        }
        */

    }

}