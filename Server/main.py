import sys

from dotenv import load_dotenv, find_dotenv
from readchar import readkey

from services.messaging import Messaging


def user_input_handler(input_key, messaging):
    # b'\x03' is the control-C key
    exit_key = b'\x03'
    if str.encode(input_key) == exit_key:
        sys.exit(0)

    sys.stdout.write('%s' % input_key)
    sys.stdout.flush()

    messaging.publish(str(input_key))


if __name__ == '__main__':
    load_dotenv(find_dotenv())

    messaging_service = Messaging()
    messaging_service.wait_ready()

    while True:
        try:
            input_key = readkey()
            user_input_handler(input_key, messaging_service)
        except KeyboardInterrupt:
            print('\nCtrl-C pressed. Exiting..')
            sys.exit(0)
