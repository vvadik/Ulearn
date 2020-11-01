FROM ubuntu:18.04

RUN apt update && apt -y upgrade \
	&& apt -y install python3 \
	&& apt -y install build-essential


COPY ./app/ /app/

WORKDIR app

RUN useradd student && chmod 700 /app/tests