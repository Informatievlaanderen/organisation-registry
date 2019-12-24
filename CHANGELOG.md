# [1.9.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.8.2...v1.9.0) (2019-12-24)


### Bug Fixes

* remove hardcoded literate console logging ([1dfb24a](https://github.com/informatievlaanderen/organisation-registry/commit/1dfb24a0e10fd092561dcbdf5970011a794d10dc))


### Features

* add mutex to ftp dump ([7a5bf4c](https://github.com/informatievlaanderen/organisation-registry/commit/7a5bf4c0762e0f4fc3c36c398b04863f3933f7e9))
* add mutex to vlaanderenbenotifier ([fdac74c](https://github.com/informatievlaanderen/organisation-registry/commit/fdac74cc7b61be1992057025befaf8ace02ed6e4))

## [1.8.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.8.1...v1.8.2) (2019-12-20)

## [1.8.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.8.0...v1.8.1) (2019-12-20)


### Bug Fixes

* use OR serialization settings ([cbd9b87](https://github.com/informatievlaanderen/organisation-registry/commit/cbd9b8762dd68c2a112ca623387cc2ea2a5ec6ad))

# [1.8.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.7.0...v1.8.0) (2019-12-19)


### Bug Fixes

* make logging setup consistent, pass ILoggerFactory ([1591307](https://github.com/informatievlaanderen/organisation-registry/commit/159130755c213a7e70e5cfb0a0d457aef8449072))


### Features

* allow anonymous connection if no user provided ([37e2781](https://github.com/informatievlaanderen/organisation-registry/commit/37e2781f3aa61ecd9b19a3c3e3f03af9339a14d3))
* use mutex to prevent concurring processing in elasticsearch runner ([376f425](https://github.com/informatievlaanderen/organisation-registry/commit/376f42533b44f876457f737fa5a946fffb1f5fc8))
* use mutex to prevent concurring processing in reporting runner ([c6090b9](https://github.com/informatievlaanderen/organisation-registry/commit/c6090b977666e41bf52bce1444f2766631cf400d))

# [1.7.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.6.2...v1.7.0) (2019-12-18)


### Bug Fixes

* don't run dynamo tests unless env var is set ([387e7e2](https://github.com/informatievlaanderen/organisation-registry/commit/387e7e22e6ffe2e955a379663028befedc36bb72))


### Features

* add DistributedMutex ([eb1b876](https://github.com/informatievlaanderen/organisation-registry/commit/eb1b876f615a590f8b93b77c49856fe0b467c8fa))
* print settings on run ([9b33be5](https://github.com/informatievlaanderen/organisation-registry/commit/9b33be5287cacfdd552ea6ddb788655848e64aea))
* use mutex to prevent concurring processing ([ef0db79](https://github.com/informatievlaanderen/organisation-registry/commit/ef0db792e10eceffab369b4e3ed36ae4dd6ea0ae))

## [1.6.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.6.1...v1.6.2) (2019-12-17)


### Bug Fixes

* prevent flood of localization warnings in logging ([f592f5a](https://github.com/informatievlaanderen/organisation-registry/commit/f592f5aa11ee033631f099cac6b4b3de92e50253))

## [1.6.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.6.0...v1.6.1) (2019-12-16)


### Bug Fixes

* push scheduler to production ([fe6e5ad](https://github.com/informatievlaanderen/organisation-registry/commit/fe6e5ad1e9a2ffedb2b82a54add88e97906e794c))

# [1.6.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.5.1...v1.6.0) (2019-12-16)


### Features

* add scheduler to docker build ([8419123](https://github.com/informatievlaanderen/organisation-registry/commit/841912392ff76ed009b996e7d850e97daabb9df8))

## [1.5.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.5.0...v1.5.1) (2019-12-13)


### Bug Fixes

* configure logging to provide ILoggerFactory sooner ([0007d34](https://github.com/informatievlaanderen/organisation-registry/commit/0007d34c1f4db89a07fa8e5613ba6b4338a5389e))
* copy any files in workspace ([cc3625f](https://github.com/informatievlaanderen/organisation-registry/commit/cc3625f68b68ff20fe0a610025509a18d6ced7c2))
* create semver file manually ([46c832b](https://github.com/informatievlaanderen/organisation-registry/commit/46c832be4eda2304e9db54ae575e600424a8529d))
* create workspace dir ([97a0e17](https://github.com/informatievlaanderen/organisation-registry/commit/97a0e170c6c61bfcfdea32d52ef2a8a5a62cfa44))
* use correct syntax ([f006587](https://github.com/informatievlaanderen/organisation-registry/commit/f006587c071bd76be5e55db3f7cbad72402f6a2d))

# [1.5.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.4.0...v1.5.0) (2019-12-13)


### Features

* default log levels to Information ([855578b](https://github.com/informatievlaanderen/organisation-registry/commit/855578bc76b33df23d5222b43c95101a44993689))

# [1.4.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.5...v1.4.0) (2019-12-12)


### Features

* add scheduler image ([272252e](https://github.com/informatievlaanderen/organisation-registry/commit/272252ed3659320c2b9fbab216684320f5ab90f2))

## [1.3.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.4...v1.3.5) (2019-12-11)


### Bug Fixes

* rename roles consistently ([9abae8b](https://github.com/informatievlaanderen/organisation-registry/commit/9abae8b8778b477756c12a129929c27031179587))

## [1.3.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.3...v1.3.4) (2019-12-10)


### Bug Fixes

* add missing kboNumber form control ([53a81f2](https://github.com/informatievlaanderen/organisation-registry/commit/53a81f203e672ab95926b79a776c6c2ce9b2a58c))

## [1.3.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.2...v1.3.3) (2019-12-09)


### Bug Fixes

* redirect using $http_host ([c6b76db](https://github.com/informatievlaanderen/organisation-registry/commit/c6b76dba19d68d5f8b6755bf09c9e7d19c7c5a7d))

## [1.3.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.1...v1.3.2) (2019-12-06)


### Bug Fixes

* coalesce empty string for FormalFrameworkName ([6d0bd70](https://github.com/informatievlaanderen/organisation-registry/commit/6d0bd70a5e584f2f0ec4cf0d3204f83d883d9516))

## [1.3.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.0...v1.3.1) (2019-12-06)


### Bug Fixes

* use serilog values for logging ([28c3b85](https://github.com/informatievlaanderen/organisation-registry/commit/28c3b8507685124c1ff2f1d0fafa9d3a914e806a))

# [1.3.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.2.0...v1.3.0) (2019-12-06)


### Features

* also oic exception log to logger ([ab72bf6](https://github.com/informatievlaanderen/organisation-registry/commit/ab72bf6733adf2c5a7d9112e423ed388dd35a874))

# [1.2.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.1.1...v1.2.0) (2019-12-06)


### Features

* increase log level ([9b65709](https://github.com/informatievlaanderen/organisation-registry/commit/9b65709f06fa2cfe73adc3a127e88dd1029f1ed2))
* provide more info on oic exchange failure ([056115d](https://github.com/informatievlaanderen/organisation-registry/commit/056115d3f9a2415924092e7969d134a641325a79))

## [1.1.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.1.0...v1.1.1) (2019-12-06)


### Bug Fixes

* push docs to nuget DC-24 ([937d911](https://github.com/informatievlaanderen/organisation-registry/commit/937d911d81b4b4817b1f5e014840e7650611ea3a))
* rename listview ([1ac6fb6](https://github.com/informatievlaanderen/organisation-registry/commit/1ac6fb6ac5edabbd47a7f92dc4e5d1889af1c969))

# [1.1.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.6...v1.1.0) (2019-12-03)


### Bug Fixes

* add paket.template for public api DC-24 ([fad7259](https://github.com/informatievlaanderen/organisation-registry/commit/fad72598d0c3b80018b18a25553c5fe17bcd3b78))
* add paket.template for public api DC-24 ([20d4971](https://github.com/informatievlaanderen/organisation-registry/commit/20d4971e11cc7a78c5da3f823e65aadf25c01f13))
* push docker containers to production ([f9b344f](https://github.com/informatievlaanderen/organisation-registry/commit/f9b344f775e314e2c00708cc26ac983f8f5c682e))


### Features

* dockerize AgentschapZorgEnGezondheid.FtpDump DC-7 ([fbbc877](https://github.com/informatievlaanderen/organisation-registry/commit/fbbc87702ec0e860695f86f0743260c6c19b24ba))
* dockerize ElasticSearch.Projections DC-9 ([15d7f0b](https://github.com/informatievlaanderen/organisation-registry/commit/15d7f0b96ce8bda71146c8e80e6f1c10e090e2a9))
* dockerize Projections.Delegations DC-10 ([e4787d3](https://github.com/informatievlaanderen/organisation-registry/commit/e4787d3de858681593de72e2e63feff5cabbbb27))
* dockerize Projections.Reporting DC-11 ([a63ba9a](https://github.com/informatievlaanderen/organisation-registry/commit/a63ba9a0a26b76526f6835e8f02d4319a5a589d5))
* dockerize VlaanderenBeNotifier DC-8 ([77d364e](https://github.com/informatievlaanderen/organisation-registry/commit/77d364e86bd2bc1d7bd83abddfdc7a44a6bd2088))

## [1.0.6](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.5...v1.0.6) (2019-12-03)


### Bug Fixes

* use correct header names ([e622c42](https://github.com/informatievlaanderen/organisation-registry/commit/e622c42330441b0529b7b209fcf527f9144ea9b2))

## [1.0.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.4...v1.0.5) (2019-12-03)


### Bug Fixes

* use correct api url ([c49bfb8](https://github.com/informatievlaanderen/organisation-registry/commit/c49bfb8a01816e932a8fc4c636bcb30eb4a5ee05))

## [1.0.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.3...v1.0.4) (2019-12-03)


### Bug Fixes

* correct title before angular loads ([92dc130](https://github.com/informatievlaanderen/organisation-registry/commit/92dc130e20a38f9a491dbd6e2e9f6fb4a02939d7))

## [1.0.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.2...v1.0.3) (2019-12-02)


### Bug Fixes

* use 9xxx port range ([b882889](https://github.com/informatievlaanderen/organisation-registry/commit/b8828897100dc8fd9803ddc3f6a74346a82cf7a8))

## [1.0.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.1...v1.0.2) (2019-12-02)


### Bug Fixes

* dont push non existent docker images ([0cbe6c0](https://github.com/informatievlaanderen/organisation-registry/commit/0cbe6c0add7c8a8274b9dc85e3443e1e169d07c9))

## [1.0.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.0...v1.0.1) (2019-12-02)


### Bug Fixes

* dont push non existent nugets ([6a33912](https://github.com/informatievlaanderen/organisation-registry/commit/6a339127e77b9b6c21a8f68b5bb3ac546e686db6))

# 1.0.0 (2019-12-02)


### Bug Fixes

* add circleci support ([92ba7e3](https://github.com/informatievlaanderen/organisation-registry/commit/92ba7e39775744717dbcfcbcc1c06a2226735c9d))
* add json compact serilog ([ec81271](https://github.com/informatievlaanderen/organisation-registry/commit/ec8127198b000231edb45d3bcbe319b4dcad3d2f))
* add missing values for tests ([8555520](https://github.com/informatievlaanderen/organisation-registry/commit/85555201e1b8f9a0de92beb715b253cdbde4c164))
* copy appsettings ([83155fa](https://github.com/informatievlaanderen/organisation-registry/commit/83155faae9e3c94fcf931ef1e12dd35ddcb9defe))
* make sure Url.Action still works ([80c0177](https://github.com/informatievlaanderen/organisation-registry/commit/80c01776a02804f0e57f0fd532eaa2dd7794c25e))
* only build api and site ([1090e3d](https://github.com/informatievlaanderen/organisation-registry/commit/1090e3d38dea520cda4c7aed28d9dbd126d852e4))
* point to correct default api endpoint ([58b87f9](https://github.com/informatievlaanderen/organisation-registry/commit/58b87f932e9186412f32d6c1fda6e933348e02ba))
* redirect oic to hash oic ([d468208](https://github.com/informatievlaanderen/organisation-registry/commit/d468208238271680802b04e10b95381cdeaa5d4f))
* redirect to idp on auth request ([1e2da40](https://github.com/informatievlaanderen/organisation-registry/commit/1e2da407f6f303240b6f1c1886ad50a6bfe0f004))
* remap port of sql server ([9d15098](https://github.com/informatievlaanderen/organisation-registry/commit/9d1509830f57d04ac5b5edacc242aa4ad5b71261))
* remove unnecessary comma ([edfe11b](https://github.com/informatievlaanderen/organisation-registry/commit/edfe11b9409ed4d865d717f22c1f49d71bf68c0e))
* rename ef context ([c8151e3](https://github.com/informatievlaanderen/organisation-registry/commit/c8151e3657d8a89f5a60425763b370a13d1954f5))
* require appsettings ([1db0710](https://github.com/informatievlaanderen/organisation-registry/commit/1db0710b67e99b8c0b3b7637d002b7846dd50eb8))
* run sql on default port ([bd60593](https://github.com/informatievlaanderen/organisation-registry/commit/bd6059385070a7f0d03dca46f139ed99086be6f4))
* run tests against localhost ([0ba8bf1](https://github.com/informatievlaanderen/organisation-registry/commit/0ba8bf124a5d37310e368bbe364f504419570878))
* see if build works without pack ([bd4f40a](https://github.com/informatievlaanderen/organisation-registry/commit/bd4f40a54f56197eb589aec2b3589d364defaeac))
* see if build works without tests ([df5f0c8](https://github.com/informatievlaanderen/organisation-registry/commit/df5f0c8b82c6586d2bec0f8cd76e7b9f7fd5a070))
* title is Wegwijs ([eda763a](https://github.com/informatievlaanderen/organisation-registry/commit/eda763af91313e9b2ecb4e3581843044bedc9726))
* use different connstrings for fixtures ([babddd0](https://github.com/informatievlaanderen/organisation-registry/commit/babddd06712016e7b184c1687453b40eb84d3dec))
* use hasAnyOfRoles ([14d54c4](https://github.com/informatievlaanderen/organisation-registry/commit/14d54c480fcc0ef63209bec676a5565be47f7a40))
* use local docker db as default ([41da3db](https://github.com/informatievlaanderen/organisation-registry/commit/41da3db76489d9b1b9a63e1cbb3329cfb72117b4))
* use nullable GUIDs for optional values ([d756739](https://github.com/informatievlaanderen/organisation-registry/commit/d756739cd24a0373db4ed5bd74e5937b3136f64e))
* use staging uris as default ([15d9d6c](https://github.com/informatievlaanderen/organisation-registry/commit/15d9d6ce0f5e9de69e03ae4f9834b48ed87c27e9))


### Features

* add agentschap zorg en gezondheid ([bf77a0b](https://github.com/informatievlaanderen/organisation-registry/commit/bf77a0bd9b34ea640e153e753f8cce3ba27b24df))
* add api ([7289d19](https://github.com/informatievlaanderen/organisation-registry/commit/7289d19ad57ad2e7eaff12984ae91819490a53ff))
* add command timeout ([58348f7](https://github.com/informatievlaanderen/organisation-registry/commit/58348f7c88097174bac5193a7652c12976b23220))
* add compensating action runner, not in sln! ([62423a8](https://github.com/informatievlaanderen/organisation-registry/commit/62423a84b8720b66e04c8683aecfbdfa9e77a5f1))
* add configuration db ([511a8f8](https://github.com/informatievlaanderen/organisation-registry/commit/511a8f8fe79fc4a0118b5f97cae65129d816dbb4))
* add default cors ([143ef58](https://github.com/informatievlaanderen/organisation-registry/commit/143ef58734af15df85cac9b7f7cee2b04ecba4ed))
* add default datadog settings ([77d0f3b](https://github.com/informatievlaanderen/organisation-registry/commit/77d0f3bac1d733ae4696e74e674ce32432de8d54))
* add delegation projections ([5c70117](https://github.com/informatievlaanderen/organisation-registry/commit/5c7011711956e9914ae6c1de74047ac166ae7304))
* add domain ([b913220](https://github.com/informatievlaanderen/organisation-registry/commit/b913220394ecec959f9b9ea405c844f708f967ac))
* add elasticsearch ([e4434bf](https://github.com/informatievlaanderen/organisation-registry/commit/e4434bf8277bb1dca6369d5adb4a8b364eef9a2c))
* add es projections ([f61c4ca](https://github.com/informatievlaanderen/organisation-registry/commit/f61c4cabf89c4501e0513e9757050530f7c41a88))
* add event rewriter, not in sln! ([2164dd0](https://github.com/informatievlaanderen/organisation-registry/commit/2164dd0881e46b69310640a979ff69d48a6d8a7e))
* add file splitter ([70caa4b](https://github.com/informatievlaanderen/organisation-registry/commit/70caa4b492579ae2c6c18395962a88781a8ea140))
* add infrastructure ([803e7e6](https://github.com/informatievlaanderen/organisation-registry/commit/803e7e6b66a939dc4823dbd2bf69487f7c17abff))
* add magda wcf ([607e132](https://github.com/informatievlaanderen/organisation-registry/commit/607e132a1e24755a321d9253a7842436a1d55a6d))
* add reporting projections ([355a4bb](https://github.com/informatievlaanderen/organisation-registry/commit/355a4bbf011f4a2c52dcd14e3886a55e97ffac04))
* add sql projection tables ([96a9a5d](https://github.com/informatievlaanderen/organisation-registry/commit/96a9a5deceff68a9d58c2f14f0d88147ecad361d))
* add tests ([eaede7f](https://github.com/informatievlaanderen/organisation-registry/commit/eaede7f7cb830aabb7fd631e2f3818542cb39c9a))
* add ui ([b7069b5](https://github.com/informatievlaanderen/organisation-registry/commit/b7069b58c92a0740beee05ab577d41da7ce3e0cb))
* add upgrader for namespace move ([d62fde8](https://github.com/informatievlaanderen/organisation-registry/commit/d62fde86b674118be87b421920f320399b5cc023))
* add vlaanderen be notifier ([30e0284](https://github.com/informatievlaanderen/organisation-registry/commit/30e028485b520046ee6b29ba17627e0c9fee62fe))
* developer is always 'in role' ([0f17c3d](https://github.com/informatievlaanderen/organisation-registry/commit/0f17c3de8e5efa22fde5b69e231f93bcf2997b4b))
* remove appInsights ([bfd9053](https://github.com/informatievlaanderen/organisation-registry/commit/bfd9053f30358e4c662da5e9db743b7100c9e9e2))
* remove unused AuthSettings ([caa77bc](https://github.com/informatievlaanderen/organisation-registry/commit/caa77bc6c6298b4c34799de51bb2ed7e6086b651))
* serialize events by full name ([456b11f](https://github.com/informatievlaanderen/organisation-registry/commit/456b11ff83d301093f9ed7c3a78c9f72b907f047))
* use empty cert when configured cert can't be found ([474b77f](https://github.com/informatievlaanderen/organisation-registry/commit/474b77f91eb64d361af418e3d60815ff0e30f620))
