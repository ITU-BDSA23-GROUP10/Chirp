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
The User activities diagram shows what is possible for a user to do on our Chirp application. Both when authenticated and unauthenticated.

//TODO: Add user activities activity diagram

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment  
The group employed the use of Github Workflows/Actions to build, test, release and deploy the app to Azure. The UML activity diagrams below show how each of the Workflows work 


## Team work

//TODO: Add picture of project board 

Above is an image of the project board for our chirp project. While the vast majority of issues are done and closed, some issues are still open.

**Overview of uncompleted issues:**
| Issue | Description | Not completed because: |
| ----------- | ----------- | ----------- |
| [#175](https://github.com/ITU-BDSA23-GROUP10/Chirp/issues/175) (Bugs) | Make playwright UI tests able to run<br> with a workflow through GitHub actions. | Problems with getting playwright test to run in “Headless” mode.<br>Because running UI tests on GitHub actions was not an important requirement for the project the issue was not prioritized. |
| [#263](https://github.com/ITU-BDSA23-GROUP10/Chirp/issues/263) (Bugs) | When running integration tests on a unix<br> based system, the tests would give a<br> `System.InvalidOperationException`. | Since the exception didn’t stop the tests from running and passing, the issue was not prioritized. |
| [#225](https://github.com/ITU-BDSA23-GROUP10/Chirp/issues/225) (Not started) | Be able to run `playwright` tests on a dockerized container of the project. | The scope of this issue was too big for the deadline of the project, and not a necessary addition. |
| [#279](https://github.com/ITU-BDSA23-GROUP10/Chirp/issues/279) (Not started) | Change general exception throws to be more specific to the reason it was thrown. | Since most of the program used general exceptions, solving this issue would take some time. Because this issue didn’t impact the performance or the functionality of the program and due to the deadline approaching, it was decide that other issues would be prioritized.  |


### Groups work activities

![Activity diagram of group workflow](images/team_workflow.svg)

[Web version of groups work activities diagram](https://www.plantuml.com/plantuml/svg/TP6_RZ8n4CHxFyMA2WfU0AJlWoXIH94eKf2Yb8QRkxFm7sUzldlzs701Yk1ovgUPNUycYMR9pgeUkW7JP-1JQyD8RM0IQ9TeB98I8nRqmY77YqBwA6OmOf18dLFKQg_JYKmYwKFkUg7GjXIOEZF0hzLgbr96zKSs8cVf5Uu0JgGPf5CodKTpJxeme249D7k2yHr5gtr1PeNv23QB2RvYarpKBRNGaXbqOEsFVNMFNAJYac94Q55KtkEEGpQsDbIghRlJzEuhBzGD8jrfuw_eBJt7HWic4hwv9gxUixLBndT6aag625_0XzFjF346Wt4sCNjeH-xoWvD5qeFYRt2IzRGoZRsneFasCfy3wtaTkCF_HMsPluM5MlxxGgf7gVej1Kd-Fibpy2Yp90bHRU2RxEmF)

The group would create issues after a lecture, based on the project work given and tasks that needed to be solved. New issues where added to the “Not started” column on the project board or “Bugs” if the issue pertained to a bug.

How work was conducted on the issues, can be seen in the diagram above [team workflow diagram].

Assigned group members take responsibility for updating the issue and the project board.

The amount of group members on one issue would vary depending on the issue. Smaller or more specialized issues would often only be assigned one group member. A specialized issue could be one that pertained to a subject that one group member was significantly more experience in than other members were.

To automate closing and moving of issues on the project board when a pull-request for an issues was merged, the group used GitHub keywords like “Resolves” and “Closes” with links to the issues.



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