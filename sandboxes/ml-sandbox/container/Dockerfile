FROM jupyter/datascience-notebook

USER root
RUN mkdir /app && chown jovyan /app

RUN pip install --upgrade pip && pip install --upgrade setuptools && pip install nltk tensorflow catboost xgboost lightgbm torch torchvision

USER jovyan

WORKDIR app
