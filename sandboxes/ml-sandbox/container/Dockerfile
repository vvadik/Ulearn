FROM jupyter/datascience-notebook

USER root
RUN mkdir /app && chown jovyan /app

RUN pip install --upgrade pip && pip install nltk

USER jovyan

WORKDIR app
