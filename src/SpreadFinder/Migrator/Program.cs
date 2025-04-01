// See https://aka.ms/new-console-template for more information

using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Migrator;

Thread.Sleep(3000); // лишнее, но видимо бд в докер композе не успевает нормально подняться, из-за чего рефьюзятся коннекты

var provider = Startup.Setup();
var runner = provider.GetRequiredService<IMigrationRunner>();

runner.MigrateUp();