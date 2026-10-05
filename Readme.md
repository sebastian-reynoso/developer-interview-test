# Smartwyre Developer Test Instructions

You have been selected to complete our candidate coding exercise. Please follow the directions in this readme.

Clone, **DO NOT FORK**, this repository to your account on the online Git resource of your choosing (GitHub, BitBucket, GitLab, etc.). Your solution should retain previous commit history and you should utilize best practices for committing your changes to the repository.

You are welcome to use whatever tools you normally would when coding — including documentation, libraries, frameworks, or AI tools (such as ChatGPT or Copilot).

However, it is important that you fully understand your solution. As part of the interview process, we will review your code with you in detail. You should be able to:

- Explain the design choices you made.
- Walk us through how your solution works.
- Make modifications or extensions to your code during the review.

Please note: if your submission appears to have been generated entirely by an AI agent or another third party, without your own understanding or contribution, it will not meet our evaluation criteria.

# The Exercise

In the 'RebateService.cs' file you will find a method for calculating a rebate. At a high level the steps for calculating a rebate are:

 1. Lookup the rebate that the request is being made against.
 2. Lookup the product that the request is being made against.
 2. Check that the rebate and request are valid to calculate the incentive type rebate.
 3. Store the rebate calculation.

What we'd like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principles
 - Testability
 - Readability
 - Currently there are 3 known incentive types. In the future the business will want to add many more incentive types. Your solution should make it easy for developers to add new incentive types in the future.

We’d also like you to 
 - Add some unit tests to the Smartwyre.DeveloperTest.Tests project to show how you would test the code that you’ve produced 
 - Run the RebateService from the Smartwyre.DeveloperTest.Runner console application accepting inputs (either via command line arguments or via prompts is fine)

The only specific "rules" are:

- The solution must build
- All tests must pass

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around 1 hour completing the exercise.

Feel free to use code comments to describe your changes. You are also welcome to update this readme with any important details for us to consider.

Once you have completed the exercise either ensure your repository is available publicly or contact the hiring manager to set up a private share.

# Solution notes

**Design.** `RebateService` now only orchestrates: it looks up the rebate and product, runs the checks shared by every incentive type, delegates the calculation to an `IIncentiveCalculator` (one class per incentive type, Strategy pattern) and stores the result. Data stores are injected through interfaces so the service can be unit tested.

**Adding a new incentive type** doesn't require changing `RebateService`:

1. Add the value to `IncentiveType` and `SupportedIncentiveType`.
2. Add a class implementing `IIncentiveCalculator` in `Calculators/`.
3. Register it in the runner's `ServiceCollection` (or your DI container).

**Behaviour.** The incentive rules are unchanged. The refactor fixes two `NullReferenceException`s in the original code (the rebate was used before its null check, and `FixedCashAmount` never checked the product).

**Running it.**

- Tests: `dotnet test`
- Runner: `dotnet run --project Smartwyre.DeveloperTest.Runner`. The real data stores are stubs, so the runner uses in-memory stores with sample data (e.g. rebate `REB-RATE`, product `PROD-ALL`, volume `3`).
