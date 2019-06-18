FROM node:current-alpine

# Installs latest Chromium (73) package.
RUN apk update && apk upgrade && \
    echo @edge http://nl.alpinelinux.org/alpine/edge/community >> /etc/apk/repositories && \
    echo @edge http://nl.alpinelinux.org/alpine/edge/main >> /etc/apk/repositories && \
    apk add --no-cache \
      chromium@edge=~73.0.3683.103 \
      nss@edge \
      freetype@edge \
      harfbuzz@edge \
      ttf-freefont@edge \
      git@edge

# Skip downloading Chromium when installing puppeteer. We'll use the installed package.
ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD true

COPY . /app/

WORKDIR app

# Add new user for sandbox
RUN addgroup -S pptruser && adduser -S -g pptruser pptruser \
   && mkdir -p /home/pptruser/Downloads \
   && chown -R pptruser:pptruser /home/pptruser \
   && chown -R pptruser:pptruser /app

# Run user as non privileged.
USER pptruser

# Install deps for tests.
RUN yarn

# Puppeteer v1.12.2 works with Chromium 73.
RUN yarn add --dev puppeteer@1.12.2

CMD ["node", "docker-test-runner.js"]
