# bin bash
set -e

aws ecr get-login-password --region ap-northeast-1 | docker login --username AWS --password-stdin 315408206347.dkr.ecr.ap-northeast-1.amazonaws.com
docker build -t imaizumi-temperature .
docker tag imaizumi-temperature:latest 315408206347.dkr.ecr.ap-northeast-1.amazonaws.com/imaizumi-temperature:latest
docker push 315408206347.dkr.ecr.ap-northeast-1.amazonaws.com/imaizumi-temperature:latest