QLNet
=====

QLCore is a forked of the QLNet C# library.
QLCore is a financial library written in C# for the Linux and Windows environments derived primarily from its C++ counterpart, Quantlib, which has been used as a base reference for modelling various financial instruments.

QLCore is thread-safe (or at least try to!), cross-platform and contains features not ported from Quantlib to QLNet, such as:
- Default term structures
- CMS Spread instrument and pricing engine
- Finite Differences methods for vanilla and barrier options under Heston model
- Monte-Carlo Lookback engine

QLCore is developped using Visual Studio Code, .NET Core 3.1 and xUnit test framework.

## Development workflow 

###### QLNet use git flow workflow.

Instead of a single master branch, this workflow uses two branches to record the history of the project. 
The *master* branch stores the official release history, and the *develop* branch serves as an integration branch for features.
The *develop* branch will also contain the complete history of the project.

###### Features 

To contribute features, you should clone the repository, create a tracking branch for develop and create the feature:

```
git clone https://github.com/OpenDerivatives/QLCore.git
git checkout -b develop origin/develop
git checkout -b some-feature develop
```

When the feature is ready, you can make a pull request to merge that feature into *develop*. 
Note that features will never be merged directly into *master*.

###### Releases

When a release is ready, we fork a release branch from *develop*. Creating this branch starts the next release cycle, 
so no new features can be added after this point; only bug fixes, documentation generation, and other release-oriented tasks go in this branch. 
Once it's ready to ship, the release gets merged into *master* and tagged with a version number. 

###### HotFix

Maintenance or “hotfix” branches are used to quickly patch production releases. This is the only branch that fork directly off of *master*. 
As soon as the fix is complete, it will be merged into both *master* and *develop*, and *master* will be tagged with an updated version number.

## Acknowledgements

Thanks to all Quantlib creators and contributors.
Thanks to all QLNet creators and contributors.
Thanks to all QLCore contributors.
