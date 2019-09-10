FROM node:current-alpine

RUN apk update && apk upgrade

RUN npm install --unsafe-perm --global full-icu

ENV NODE_ICU_DATA="/usr/local/lib/node_modules/full-icu"

# Add new user for sandbox
RUN addgroup -S student && adduser -S -g student student \
    && chown -R student:student /home/student

COPY ./app/ /app/

RUN mkdir -p /source \
    && chown -R student:student /app

WORKDIR app

# Run user as non privileged.
USER student

# Install deps for tests.
RUN yarn
