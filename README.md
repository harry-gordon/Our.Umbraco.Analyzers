# Umbraco Analyzers

This is a WIP collection of Roslyn package analyzers aimed at improving the Umbraco developer experience.

## Analyzers

The list of analyzers is as follows:

- Stateful content models (`UMB1000`): checks for stateful content models that might indicate the model is [being used as a view model 💀](https://harrygordon.co.uk/blog/content-model-best-practices/).
- Missing using keyword (`UMB1010`): checks for `using` keyword when `EnsureUmbracoContext` is called.

# Additional analyzers

I'd like to keep expanding this collection of analyzers, so if you have any ideas give me a shout on Discord or [Mastodon](https://umbracocommunity.social/@harrygordon). Alternatively, create a PR for this project. Here are the next few issues I'm planning to write analyzers for:

- Other cases where disposal of Umbraco resources is critical
- Static instances of Umbraco Helper
