---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2023 Group `10`
author:
- "Theis Per Holm <thph@itu.dk>"
- "August Kofoed Brandt <aubr@itu.dk>"
# Add your names here guys
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

Here comes a description of our domain model.

![Illustration of the _Chirp!_ data model as UML class diagram.](docs/images/domain_model.png)

## Architecture — In the small

## Architecture of deployed application

## User activities

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment  
The group employed the use of Github Workflows/Actions to build, test, release and deploy the app to Azure. The UML activity diagrams below show how each of the Workflows work 


## Team work

//TODO: Add picture of project board 

//TODO: Add text about missing issues and features

//TODO: Add team workflow diagram

The group would create issues after a lecture, based on the project work given and tasks that needed to be solved. New issues where added to the “Not started” column on the project board or “Bugs” if the issue pertained to a bug.

How work was conducted on the issues, can be seen in the diagram [team workflow diagram]. Once an issue was ready to be worked, group members would be assigned to the issue and take responsibility for solving the issue and updating it on the project board

In the start of the project the issues where quite simple. They had a title, some extra description, if needed, and acceptance criteria in a normal list format. However, after some time the group started adding acceptance criteria lists with checkboxes that could be checked off when an acceptance criteria was met. This allowed other group members to follow how far along work on issues was

The amount of group members on one issue would vary depending on the issue. Smaller or more specialized issues would often only be assigned one group member. A specialized issue could be one that pertained to a subject that one group member was significantly more experience in than other members were.

Group members working on an issue would create a feature branch, where the development on testing of the solution to the issue would be done. Once issue’s group considered the solution done, they would create a pull-request on GitHub, that the group members that had not been a part of the issue would then review. The other group members then had the opportunity request changes to the solution or ask question to the other issue’s group about the solution. When all request changes where addressed, all checks were passing, and the group had approved for merge, then the pull-request was merged into main, and the issue was closed

To automate closing and moving of issues on the project board when a pull-request for an issues was merged, the group used GitHub keywords like “Resolves” and “Closes” with links to the issues. This automatically closes the issue, and it gets moved to the “Done” column on the project board.


## How to make _Chirp!_ work locally
For a full guide on how to run the project locally see the ReadMe.md on the public repository: [Chirp ReadMe.md](https://github.com/ITU-BDSA23-GROUP10/Chirp/blob/main/README.md)  

<!--![Diagram Image Link](./report_diagrams/UML_activity_diagrams/build_test_UML.puml)-->


## How to run test suite locally


# Ethics

## License  
The group has chosen the MIT open source software license 


>MIT License
>
>Copyright (c) [year] [fullname]
>
>Permission is hereby granted, free of charge, to any person obtaining a copy
>of this software and associated documentation files (the "Software"), to deal
>in the Software without restriction, including without limitation the rights
>to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
>copies of the Software, and to permit persons to whom the Software is
>furnished to do so, subject to the following conditions:
>
>The above copyright notice and this permission notice shall be included in all
>copies or substantial portions of the Software.
>
>THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
>IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
>FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
>AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
>LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
>OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
>SOFTWARE.  

//TODO: figure out how we want to source from things
Source: [Github choose a license site](https://choosealicense.com/licenses/mit/)

The group chose this license as it was a good fit for the groups requirements of an open source license in that it basically has no restrictions for any end user or somebody who wants to work with the codebase. We also sign off any warranty or liability which is great for a small group project that we more than likely wont want to take further in the future.

## LLMs, ChatGPT, CoPilot, and others
The use of LLMs like ChatGPT and Copilot has been documented on github commits as a co-author when used. You can see the number of these commits on the page linked here: [ChatGPT Co-authored commits](https://github.com/ITU-BDSA23-GROUP10/Chirp/graphs/contributors). Sadly the page that shows the actual commits doesn't have the commits that it contributed on as these were done on separate branches whose commits seem to not carry over to the main branch's working tree. 