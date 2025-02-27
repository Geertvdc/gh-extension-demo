# gh-extension-demo
This repo describes how to create extensions for GitHub Copilot using .Net.

There are 2 examples. Both using C# and .Net to implement a GitHub Copilot Extension to demonstrate the different options how to build extensions

## Skillset Example 
The first example is a skillset that exposes the basic weather api that comes out of the box when generating a new .Net API and exposes that as a skill to GitHub copilot to show how you can turn any API into a GitHub Copilot Extension Skillset.

![Skillset Example](img/skillset.png)

## Agent Example
The second example is an agent implementation of a GitHub Copilot Extension. This example shows how you can build an agent where you have full control of the complete interactions with Copilot and LLM models. In this example we use C# to help us find the right colleague when working on a specific technology.

![Agent Example](img/agent.png)


### Debugging
You'll need the Github CLI
```
brew install gh
```

Then you need to authenticate with the GitHub CLI
```
gh auth login --web -h github.com
```

After that you can install the debugger extension
```
gh extension install github.com/copilot-extensions/gh-debug-cli
```

Then you'll need the github token to authenticate to your agent:
```
GITHUB_TOKEN=$(gh auth token)
```

then finally you can actually start the debugger
```
gh debug-cli chat --url http://localhost:5125/agent --token $GITHUB_TOKEN
```
