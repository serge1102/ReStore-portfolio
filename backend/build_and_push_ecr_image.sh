set -e

ECRREPOSITRY="restore-dev"

REGION=$(aws configure get region)
ACCOUNTID=$(aws sts get-caller-identity --output text --query Account)
aws ecr get-login-password | docker login --username AWS \
  --password-stdin ${ACCOUNTID}.dkr.ecr.${REGION}.amazonaws.com

docker build -f ./Dockerfile -t ${ECRREPOSITRY}:latest .
docker tag ${ECRREPOSITRY}:latest \
  ${ACCOUNTID}.dkr.ecr.${REGION}.amazonaws.com/${ECRREPOSITRY}:latest
docker push ${ACCOUNTID}.dkr.ecr.${REGION}.amazonaws.com/${ECRREPOSITRY}:latest