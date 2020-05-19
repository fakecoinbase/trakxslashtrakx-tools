using System;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Trakx.IndiceManager.Client.Tests
{
    public abstract class ComponentTest<T> : ComponentTestFixture where T : class, IComponent
    {
        #nullable  disable
        protected IRenderedComponent<T> Component;
        #nullable enable

        public async Task Dispatch<TValue>(EventCallback<TValue> callback, TValue value)
        {
            await callback.InvokeOnDispatcherOf(Component, value);
        }
        
        public async Task<U> Dispatch<U>(Func<U> function)
        {
            return await function.InvokeOnDispatcherOf(Component);
        }

        public async Task Dispatch(Action action)
        {
            await action.InvokeOnDispatcherOf(Component);
        }
    }

    public static class RenderedComponentExtensions
    {
        public static async Task InvokeOnDispatcherOf<TComponent, TValue>(this EventCallback<TValue> callback, IRenderedComponent<TComponent> component, TValue value)
            where TComponent : class, IComponent
        {
            await component.TestContext.Renderer.Dispatcher.InvokeAsync(async () => await callback.InvokeAsync(value));
        }

        public static async Task<T> InvokeOnDispatcherOf<TComponent, T>(this Func<T> function, IRenderedComponent<TComponent> component)
            where TComponent : class, IComponent
        {
            return await component.TestContext.Renderer.Dispatcher.InvokeAsync(function.Invoke);
        }

        public static async Task InvokeOnDispatcherOf<TComponent>(this Action action, IRenderedComponent<TComponent> component)
            where TComponent : class, IComponent
        {
            await component.TestContext.Renderer.Dispatcher.InvokeAsync(action.Invoke);
        }
    }
}