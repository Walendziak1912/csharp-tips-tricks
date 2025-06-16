using AppPlusSQL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace AppPlusSQL.Persistence.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Teams.Any())
            {
                return;
            }

            // Faker dla Teams
            var teamFaker = new Faker<Team>("pl")
                .RuleFor(t => t.Name, f => f.Company.CompanyName())
                .RuleFor(t => t.Members, f => new List<Member>());

            // Generowanie 10 teamów
            var teams = teamFaker.Generate(30);
            context.Teams.AddRange(teams);
            context.SaveChanges();

            // Pobieranie teamów z bazy (z Id)
            var savedTeams = context.Teams.ToList();

            // Faker dla Members
            var memberFaker = new Faker<Member>("pl")
                .RuleFor(m => m.Name, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.TeamId, f => f.PickRandom(savedTeams).Id)
                .RuleFor(m => m.Activities, f => new List<Activity>());

            // Generowanie 50 memberów dla każdego teamu (500 total)
            var members = new List<Member>();
            foreach (var team in savedTeams)
            {
                var teamMembers = memberFaker
                    .RuleFor(m => m.TeamId, f => team.Id)
                    .Generate(150);
                members.AddRange(teamMembers);
            }

            context.Members.AddRange(members);
            context.SaveChanges();

            // Pobieranie memberów z bazy (z Id)
            var savedMembers = context.Members.ToList();

            // Faker dla Activities
            var activityFaker = new Faker<Activity>("pl")
                .RuleFor(a => a.Action, f => f.PickRandom("created_task", "commented", "uploaded_file", "reviewed_code", "assigned_task"))
                .RuleFor(a => a.CreatedAt, f => f.Date.Past(1)) // ostatni rok
                .RuleFor(a => a.MemberId, f => f.PickRandom(savedMembers).Id);

            // Generowanie 10 activities dla każdego membera
            var activities = new List<Activity>();
            foreach (var member in savedMembers)
            {
                var memberActivities = activityFaker
                    .RuleFor(a => a.MemberId, f => member.Id)
                    .Generate(50);
                activities.AddRange(memberActivities);
            }

            context.Activities.AddRange(activities);
            context.SaveChanges();
        }
    }
}
