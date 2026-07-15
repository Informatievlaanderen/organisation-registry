#!/usr/bin/python

import sys, os
import mimetypes, codecs
import re, collections
import requests, json
import argparse, urllib, webbrowser
import datetime

# ArgumentParser to parse arguments and options
parser = argparse.ArgumentParser()
parser.add_argument("version", help="Version to create.")
parser.add_argument('project', help="Jira project key for the page.")
parser.add_argument('-u', '--username', help='Jira username if $JIRA_USERNAME not set.')
parser.add_argument('-p', '--password', help='Jira password if $JIRA_PASSWORD not set.')
parser.add_argument('-o', '--orgname', help='Jira organisation if $JIRA_ORGNAME not set. e.g. https://XXX.atlassian.net')
parser.add_argument('-g', '--github', help='Use this option to provide a github url.')
parser.add_argument('-r', '--repo', help='Use this option to provide a repository name.')
parser.add_argument('-n', '--nossl', action='store_true', default=False, help='Use this option if NOT using SSL. Will use HTTP instead of HTTPS.')
args = parser.parse_args()

# Assign global variables
try:
  version = args.version
  project = args.project
  username = os.getenv('JIRA_USERNAME', args.username)
  password = os.getenv('JIRA_PASSWORD', args.password)
  orgname = os.getenv('JIRA_ORGNAME', args.orgname)
  github = args.github
  repo = args.repo
  nossl = args.nossl

  if version is None:
    print('Error: Version not specified by option.')
    sys.exit(1)

  if project is None:
    print('Error: Project not specified by option.')
    sys.exit(1)

  if username is None:
    print('Error: Username not specified by environment variable or option.')
    sys.exit(1)

  if password is None:
    print('Error: Password not specified by environment variable or option.')
    sys.exit(1)

  if orgname is None:
    print('Error: Org Name not specified by environment variable or option.')
    sys.exit(1)

  if github is None:
    print('Error: Github not specified by option.')
    sys.exit(1)

  if repo is None:
    print('Error: Repository not specified by option.')
    sys.exit(1)

  projectUrl = 'https://%s.atlassian.net/' % orgname
  if nossl:
    projectUrl.replace('https://','http://')

except Exception as err:
  print('\n\nException caught:\n%s ' % (err))
  print('\nFailed to process command line arguments. Exiting.')
  sys.exit(1)

# Create a new version
def createVersion(versionToCreate, projectToUse):
  print('\nCreating version...')

  url = '%s/rest/api/2/version/' % projectUrl
  description = 'Automatically created version at %s/releases/tag/v%s' % (github, versionToCreate)

  s = requests.Session()
  s.auth = (username, password)
  s.headers.update({'Content-Type' : 'application/json'})

  newVersion = { 'archived' : 'false', \
   'project' : projectToUse, \
   'name' : versionToCreate, \
   'description' : description, \
   'released' : 'false', \
   'releaseDate' : datetime.datetime.utcnow().isoformat() \
   }

  r = s.post(url, data=json.dumps(newVersion))
  r.raise_for_status()

  if r.status_code == 201:
    data = r.json()
    versionUrl = data[u'self']

    print('\nVersion created in %s with name: %s.' % (projectToUse, versionToCreate))
    print('URL: %s' % versionUrl)
  else:
    print('\nCould not create version.')
    sys.exit(1)

def main():
  print('\n\n\t\t-------------------------')
  print('\t\tJira Version Creator Tool')
  print('\t\t-------------------------\n\n')

  print('Version:\t%s' % version)
  print('Project Key:\t%s' % project)

  createVersion(version, project)

  print('\nJira Version Creator completed successfully.')

if __name__ == "__main__":
  main()
