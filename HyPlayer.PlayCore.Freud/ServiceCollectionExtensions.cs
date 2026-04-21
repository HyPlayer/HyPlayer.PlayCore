using System;

namespace HyPlayer.PlayCore.Freud
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSingleton<TService, TImpl>(this ServiceCollection sc) where TImpl : TService
        {
            sc.AddSingleton<TService, TImpl>();
        }

        public static void AddSingleton<TService>(this ServiceCollection sc, TService instance)
        {
            sc.AddSingleton(instance);
        }

        public static void AddTransient<TService, TImpl>(this ServiceCollection sc) where TImpl : TService
        {
            sc.AddTransient<TService, TImpl>();
        }
    }
}
