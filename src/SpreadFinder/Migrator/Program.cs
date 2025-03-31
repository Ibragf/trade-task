// See https://aka.ms/new-console-template for more information

using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Migrator;

var provider = Startup.Setup();
var runner = provider.GetRequiredService<IMigrationRunner>();

runner.MigrateUp();