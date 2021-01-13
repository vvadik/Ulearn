FROM haskell:8.10

RUN mkdir /app

WORKDIR app

RUN cabal v2-update
RUN cabal v2-install --lib hspec-2.7.6 QuickCheck-2.14.2
