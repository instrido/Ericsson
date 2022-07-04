import os
import time

import pika


class Messaging:
    def __init__(self):
        self.__setup_variables()
        self.__setup_environment()

    def __setup_variables(self):
        self.timeToRetry = 1  # seconds

    def __setup_environment(self):
        self.hostName = os.getenv('BROKER_HOST')
        self.queueName = os.getenv('BROKER_QUEUE')

    def wait_ready(self):
        print('Connecting to broker: %s, queue: %s' % (self.hostName, self.queueName))
        print('Waiting to connect to broker..', end='')
        while True:
            try:
                self.connection = pika.BlockingConnection(pika.ConnectionParameters(host=self.hostName))
                self.channel = self.connection.channel()
                if self.connection.is_open:
                    print('Connected!')
                    break
            except pika.exceptions.AMQPConnectionError:
                print('.', end='')
                time.sleep(self.timeToRetry)
            except Exception as e:
                print(e)
                import sys
                sys.exit(1)

    def publish(self, data):
        self.channel.basic_publish(exchange='', routing_key=self.queueName, body=data)
