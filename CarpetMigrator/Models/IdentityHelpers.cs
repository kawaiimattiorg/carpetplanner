﻿namespace CarpetMigrator.Models;

using Microsoft.EntityFrameworkCore;

public static class IdentityHelpers
{
    private static Task EnableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: true);
    private static Task DisableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: false);

    private static Task SetIdentityInsert<T>(DbContext context, bool enable)
    {
        var entityType = context.Model.FindEntityType(typeof(T));
        var value = enable ? "ON" : "OFF";
        return context.Database.ExecuteSqlRawAsync(
            $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
    }

    public static void SaveChangesWithIdentityInsert<T>(this DbContext context)
    {
        using var transaction = context.Database.BeginTransaction();
        context.EnableIdentityInsert<T>();
        context.SaveChanges();
        context.DisableIdentityInsert<T>();
        transaction.Commit();
    }
}
