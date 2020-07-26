1. Start your rabbitmq server.
2. Change Rabbit mq configuration and CSV path file in TestMessageQueue.Api/appsetting.json file.
2. Run TestMessageQueue.Api.sln with docker or IIS server by visualstudio.
3. Call api with post method, url: http://localhost:{port}/testrabbitmq/test-concurrent?n=1000&delay=1&total=10.

Note: If you used rabbitmq with docker, you would change rabbitmq hostname by container IP.