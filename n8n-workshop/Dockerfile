FROM docker.n8n.io/n8nio/n8n

USER root

# uv is required to run some stdio MCP in this workshop.
RUN apk add --no-cache uv

USER node

EXPOSE 5678
WORKDIR /home/node

ENTRYPOINT ["tini", "--", "/docker-entrypoint.sh"]