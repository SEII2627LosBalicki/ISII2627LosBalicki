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

            try {
                SeedSports(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Sports in the Database.");
            }
            
            try {
                SeedTeams(dbContext, userManager);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Teams in the Database.");
            }
            
            try {
                SeedGames(dbContext, userManager);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding games in the Database.");
            }

            try {
                var game = dbContext.Games.FirstOrDefault(g => g.Name == "Soccer Match 1");
                SeedRefereeGroups(dbContext, game);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Referee Groups in the Database.");
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {
            foreach (string roleName in roles) {
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName.ToUpper();
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            if (userManager.FindByNameAsync("tucalvitofavorito@gmail.com").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Isi", "Palazón", "tucalvitofavorito@gmail.com");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[2]).Wait(); // User role
                }
            }
            
            if (userManager.FindByNameAsync("carloskirk@gmail.com").Result == null) {
                ApplicationUser user = new ApplicationUser("2", "Carlos", "Kirk", "carloskirk@gmail.com");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[2]).Wait(); // User role
                }
            }

            if (userManager.FindByNameAsync("balicki@gmail.com").Result == null) {
                ApplicationUser user = new ApplicationUser("0", "Kryzstof", "Balicki", "balicki@gmail.com");
                user.EmailConfirmed = true;          

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();
            
                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[0]).Wait(); // Administrator role
                }
            }

            if (userManager.FindByNameAsync("referee2@uclm.es").Result == null) {
                Referee referee = new Referee("3", "Ana", "López Pérez", "referee2@uclm.es", yearsRefereeing: 3, rating: 5);
                referee.EmailConfirmed = true;

                var result = userManager.CreateAsync(referee, "RefereePass123$");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(referee, roles[1]).Wait(); // Referee role
                }
            }
            
            if (userManager.FindByNameAsync("referee1@uclm.es").Result == null) {
                Referee referee = new Referee("2", "Carlos", "Mateo Gómez", "referee1@uclm.es", yearsRefereeing: 5, rating: 4);
                referee.EmailConfirmed = true;
                
                var result = userManager.CreateAsync(referee, "RefereePass123$");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(referee, roles[1]).Wait(); // Referee role
                }
            }
        }

        

        public static void SeedRefereeGroups(ApplicationDbContext dbContext, Game? game) {
            if (game != null && !dbContext.RefereeGroups.Any(rg => rg.Name == "Referee Group 1")) {
                var refereeGroup = new RefereeGroup("Referee Group 1", game.Id, "Group of referees for Soccer Match 1", "Follow the official RFEF rules.");
                dbContext.RefereeGroups.Add(refereeGroup);
                dbContext.SaveChanges();

                var ref1 = dbContext.Referees.FirstOrDefault(r => r.UserName == "referee1@uclm.es");
                var ref2 = dbContext.Referees.FirstOrDefault(r => r.UserName == "referee2@uclm.es");

                if (ref1 != null) {
                    var assignment1 = new RefereeAssignedTo(refereeGroup.Id, ref1.Id, "Main referee", "Responsible for enforcing the rules and making final decisions during the match.");
                    dbContext.RefereeAssignedTos.Add(assignment1);
                }
                if (ref2 != null) {
                    var assignment2 = new RefereeAssignedTo(refereeGroup.Id, ref2.Id, "Assistant referee", "Responsible for assisting the main referee in enforcing the rules and making decisions during the match.");
                    dbContext.RefereeAssignedTos.Add(assignment2);
                }
                dbContext.SaveChanges();
            }
        }

         public static void SeedSports(ApplicationDbContext dbcontext) 
        {
            if (dbcontext.Sports.FirstOrDefault(s => s.Name == "Football") == null) {
                var sport = new Sport(0, "Football", "Team sport playing 11vs11", "Don't touch the ball with the hands, except the goalkeeper.") 
                { 
                    MinimumNumberofPlayers = 22,
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

        
        public static void SeedGames(ApplicationDbContext dbcontext, UserManager<ApplicationUser> userManager) 
        {
            if (dbcontext.Games.FirstOrDefault(g => g.Name == "Soccer Match 1") == null) 
            {
                var sport = dbcontext.Sports.FirstOrDefault(s => s.Name == "Football");
                var responsibleUser = userManager.FindByNameAsync("balicki@gmail.com").Result;

                if (sport != null && responsibleUser != null) 
                {
                    var game = new Game(0, "Soccer Match 1", DateTime.Now.AddDays(7), "Estadio Vallecas", "Partido amistoso inicial", sport.Id, responsibleUser.Id)
                    {
                        SportId = sport.Id,
                        ResponsibleForId = responsibleUser.Id
                    };
                    
                    dbcontext.Games.Add(game);
                }
            }
            dbcontext.SaveChanges();
        }


        public static void SeedTeams(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            if (dbContext.Teams.FirstOrDefault(t => t.Name == "ColoColo") == null)
            {
                var sport = dbContext.Sports.FirstOrDefault(s => s.Name == "Football");
                var captain = userManager.FindByNameAsync("carloskirk@gmail.com").Result;
                var invitedPlayer = userManager.FindByNameAsync("tucalvitofavorito@gmail.com").Result;

                if (sport != null && captain != null)
                {
                    var team = new Team(0, "ColoColo", 15, 18, 50, sport, captain);

                    dbContext.Teams.Add(team);
                    dbContext.SaveChanges();

                    if (invitedPlayer != null && dbContext.TeamInvitations.Find(invitedPlayer.Id, team.Id) == null)
                    {
                        var invitation = new TeamInvitation(
                            invitedPlayer.Id,
                            team.Id,
                            "Join our team",
                            invitationAccepted: true
                        );
                        dbContext.TeamInvitations.Add(invitation);
                        dbContext.SaveChanges();
                    }
                }
            }

            if (dbContext.Teams.FirstOrDefault(t => t.Name == "SanMarino") == null)
            {
                var sport = dbContext.Sports.FirstOrDefault(s => s.Name == "Padel");
                var captain = userManager.FindByEmailAsync("tucalvitofavorito@gmail.com").Result;

                if (sport != null && captain != null)
                {
                    var team = new Team(0, "SanMarino", 4, 16, 85, sport, captain);
                    dbContext.Teams.Add(team);
                    dbContext.SaveChanges();
                }
            }
        }


       

    }
}