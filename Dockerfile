FROM ubuntu
COPY ./Build/Linux /server/
WORKDIR /server

EXPOSE 7777/udp

CMD ./server.x86_64