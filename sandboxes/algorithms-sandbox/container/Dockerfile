FROM ubuntu:20.04

ENV NVM_DIR /usr/local/nvm
ENV NODE_VERSION 14.15.3
ARG DEBIAN_FRONTEND=noninteractive
ENV TZ=Europe/Ekaterinburg

RUN apt update && apt -y upgrade && apt install -y wget && apt -y install curl \
	# Python 3.8
	&& apt -y install python3.8 \ 
	# C, C++
	&& apt -y install build-essential \
	# C#
	&& wget https://packages.microsoft.com/config/ubuntu/20.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
	&& dpkg -i packages-microsoft-prod.deb \
	&& apt update && apt install -y apt-transport-https && apt install -y dotnet-sdk-5.0 \
  	&& apt install -y apt-transport-https && apt install -y dotnet-runtime-5.0 \
	# Java
	&& apt install -y openjdk-14-jdk \
	# JavaScript
	&& curl --silent -o- https://raw.githubusercontent.com/creationix/nvm/v0.31.2/install.sh | bash \
    && . $NVM_DIR/nvm.sh \
    && nvm install $NODE_VERSION \
    && nvm alias default $NODE_VERSION \
    && nvm use default

ENV NODE_PATH $NVM_DIR/v$NODE_VERSION/lib/node_modules
ENV PATH $NVM_DIR/versions/node/v$NODE_VERSION/bin:$PATH

COPY ./app/ /app/

WORKDIR app

RUN useradd student && chmod 700 /app/tests