FROM python:3.12-slim
RUN pip install --no-cache-dir pymssql==2.3.1
WORKDIR /seed
COPY seed.py .
CMD ["python3", "-u", "seed.py"]
