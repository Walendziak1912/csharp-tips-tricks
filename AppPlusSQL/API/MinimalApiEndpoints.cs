using AppPlusSQL.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AppPlusSQL.API
{
    public static class MinimalApiEndpoints
    {
        public static void MapApiEndpoints(this WebApplication app)
        {
            // Strona główna - przekierowanie do Swagger
            app.MapGet("/", () => Results.Redirect("/swagger"))
                .WithName("Home")
                .WithOpenApi();

            // Endpointy API używające repozytoriów
            app.MapGet("/api/members/last-activities", async (IActivityRepository activityRepo) =>
            {
                var lastActivitiesPerUser = await activityRepo.GetLastActivitiesPerUserAsync();
                return Results.Ok(new 
                { 
                    Message = "Ostatnie aktywności użytkowników",
                    Data = lastActivitiesPerUser,
                    Count = lastActivitiesPerUser.Count()
                });
            }).WithName("GetLastActivities").WithOpenApi();

            app.MapGet("/api/members", async (IMemberRepository memberRepo) =>
            {
                var members = await memberRepo.GetAllMembersAsync();
                return Results.Ok(members);
            }).WithName("GetMembers").WithOpenApi();

            app.MapGet("/api/members/{id:int}", async (int id, IMemberRepository memberRepo) =>
            {
                var member = await memberRepo.GetMemberByIdAsync(id);
                return member != null ? Results.Ok(member) : Results.NotFound($"Członek o ID {id} nie został znaleziony");
            }).WithName("GetMemberById").WithOpenApi();

            app.MapGet("/api/activities", async (IActivityRepository activityRepo) =>
            {
                var activities = await activityRepo.GetAllActivitiesAsync();
                return Results.Ok(activities);
            }).WithName("GetActivities").WithOpenApi();

            app.MapGet("/api/activities/{id:int}", async (int id, IActivityRepository activityRepo) =>
            {
                var activity = await activityRepo.GetActivityByIdAsync(id);
                return activity != null ? Results.Ok(activity) : Results.NotFound($"Aktywność o ID {id} nie została znaleziona");
            }).WithName("GetActivityById").WithOpenApi();

            app.MapGet("/api/members/{memberId:int}/activities", async (int memberId, IActivityRepository activityRepo) =>
            {
                var activities = await activityRepo.GetActivitiesByMemberIdAsync(memberId);
                return Results.Ok(activities);
            }).WithName("GetActivitiesByMember").WithOpenApi();
        }
    }
}
