namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Referee", "User" };

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

            try 
            {
                SeedSportsGamesAndReferees(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Sports, Games and Referees in the Database.");
            }
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
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }

            if (userManager.FindByNameAsync("referee1@uclm.es").Result == null) {
                Referee referee = new Referee("2", "Carlos", "Mateo Gómez", "referee1@uclm.es", yearsRefereeing: 5, rating: 4);
                referee.EmailConfirmed = true;

                var result = userManager.CreateAsync(referee, "RefereePass123$");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //referee role
                    userManager.AddToRoleAsync(referee, roles[1]).Wait();
                }
            }

            if (userManager.FindByNameAsync("referee2@uclm.es").Result == null) {
                Referee referee = new Referee("3", "Ana", "López Pérez", "referee2@uclm.es", yearsRefereeing: 3, rating: 5);
                referee.EmailConfirmed = true;

                var result = userManager.CreateAsync(referee, "RefereePass123$");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //referee role
                    userManager.AddToRoleAsync(referee, roles[1]).Wait();
                }
            }
        }

        public static void SeedSportsGamesAndReferees(ApplicationDbContext dbContext) {
            var sport = dbContext.Sports.FirstOrDefault(s => s.Name == "Soccer");
            if (sport == null) {
                sport = new Sport("Soccer"); {
                    sport.MinimumNumberofPlayers = 11;
                    sport.NumberofReferees = 3;
                    sport.Description = "Soccer is a team sport played between two teams of eleven players with a spherical ball.";
                    sport.BasicRules = "The objective of the game is to score by getting the ball into the opposing goal.";
                }

                dbContext.Sports.Add(sport);
                dbContext.SaveChanges();
            }

            var game = dbContext.Games.FirstOrDefault(g => g.Name == "Soccer Match 1");
            var user = dbContext.Users.FirstOrDefault(u => u.UserName == "elena@uclm.es");

            if (game == null)
            {
                game = new Game("Soccer Match 1", DateTime.Now.AddDays(7), "José Copete", "First soccer match of the season");
                {
                    game.Sport = sport;
                    game.ResponsibleFor = user;
                };
                dbContext.Games.Add(game);
                dbContext.SaveChanges();
            }

            if (!dbContext.RefereeGroups.Any(rg => rg.Name == "Soccer Match 1 Referee Group"))
            {
                var refereeGroup = new RefereeGroup("Referee Group 1", game.Id, "Group of referees for Soccer Match 1", "Follow the official RFEF rules.");
                dbContext.RefereeGroups.Add(refereeGroup);
                dbContext.SaveChanges();

                var ref1 = dbContext.Referees.FirstOrDefault(r => r.UserName == "referee1@uclm.es");
                var ref2 = dbContext.Referees.FirstOrDefault(r => r.UserName == "referee2@uclm.es");

                if (ref1 != null)
                {
                    var assignment1 = new RefereeAssignedTo(refereeGroup.Id, ref1.Id, "Main referee for Soccer Match 1", "Responsible for enforcing the rules and making final decisions during the match.");
                    dbContext.RefereeAssignedTos.Add(assignment1);
                }
                if (ref2 != null)
                {
                    var assignment2 = new RefereeAssignedTo(refereeGroup.Id, ref2.Id, "Assistant referee for Soccer Match 1", "Responsible for assisting the main referee in enforcing the rules and making decisions during the match.");
                    dbContext.RefereeAssignedTos.Add(assignment2);
                }
            }
        }

    }
}