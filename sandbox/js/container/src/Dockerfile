FROM node:current-alpine

# Add new user for sandbox
RUN addgroup -S student && adduser -S -g student student \
   && chown -R student:student /home/student

COPY . /app/

RUN chown -R student:student /app

WORKDIR app

# Run user as non privileged.
USER student

# Install deps for tests.
RUN yarn

CMD ["node", "docker-test-runner.js"]
