# Ericsson Technical Task
Project demonstrates client-side mirroring the server-side user-inputs via inter-container communication means.
![](https://github.com/instrido/Ericsson/raw/master/demo.gif)

## Overview

- Server-Side: Python using pika, readchar and python-dotenv libraries
- Client-Side: C# using dependency injection and Rabbitmq client libraries.
- Docker used as containerizing solution

## Installation

This project assumes `Windows OS` as the target machine with `Docker` installed to demo the application.

- Clone the repository.
- Open two terminals or split one side-by-side.
- On both terminals, change to the cloned directory.
- Run `docker-compose run --rm server` on one terminal.
- Wait until `message_broker` container is created(shared dependency), then run `docker-compose run --rm client`.
- Both sides are up when both terminals indicate by saying `Connected!`.
- Switch to the server-side and type something. 
- Observe that the client terminal mirrors user inputs on the server side.

## Todo
- Unit tests + Integration Tests.
- Handle any/if edge cases.
- Decouple message broker service from rabbitmq implementationd details.