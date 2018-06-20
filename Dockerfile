FROM node:10-alpine

# Installs latest Chromium (64) package.
RUN apk update && apk upgrade && \
    echo @edge http://nl.alpinelinux.org/alpine/edge/community >> /etc/apk/repositories && \
    echo @edge http://nl.alpinelinux.org/alpine/edge/main >> /etc/apk/repositories && \
    apk add --no-cache \
      chromium@edge \
      nss@edge \
      git@edge

# Skip downloading Chromium when installing puppeteer. We'll use the installed package.
ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD true

COPY . /app/

WORKDIR app

# Install deps for tests.
RUN yarn

# Use Puppeteer 0.13.0 b/c it bundles Chromium 64.
RUN yarn add --dev puppeteer@0.13.0

# Add new user for sandbox
RUN addgroup -S pptruser && adduser -S -g pptruser pptruser \
    && mkdir -p /home/pptruser/Downloads \
    && chown -R pptruser:pptruser /home/pptruser \
    && chown -R pptruser:pptruser /app

# Run user as non privileged.
USER pptruser

CMD ["node", "docker-test-runner.js"]
